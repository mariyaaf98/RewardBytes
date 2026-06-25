using AppWeaver.Mediator.Interfaces;

using AppWeaver.Results;

using Microsoft.EntityFrameworkCore;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Infrastructure.Security.Keycloak;

namespace BytesRewards.Service.Users.Features.GetUsers;

public sealed class GetUsersQueryHandler(
    ApplicationDbContext dbContext,
    IKeycloakAdminService keycloakAdminService
)
    : IQueryHandler<GetUsersQuery, Result<List<UserResponse>>>
{
    public async ValueTask<Result<List<UserResponse>>> Handle(
    GetUsersQuery request,
    CancellationToken ct)
    {
        var users = await dbContext.Users
            .Include(x => x.Department)
            .Include(x => x.Designation)
            .ToListAsync(ct);

        // Load all wallets in one query and index by UserId for O(1) lookup
        var wallets = await dbContext.Wallets
            .ToDictionaryAsync(w => w.UserId, w => w.AvailableBytes, ct);

        var token =
            await keycloakAdminService
                .GetAdminTokenAsync(ct);

        var response = new List<UserResponse>();

        foreach (var user in users)
        {
            var roleName = string.Empty;

            if (!string.IsNullOrWhiteSpace(user.KeycloakUserId))
            {
                roleName =
                    await keycloakAdminService
                        .GetUserRoleAsync(
                            token,
                            user.KeycloakUserId,
                            ct);
            }

            response.Add(
                new UserResponse
                {
                    Id = user.Id,

                    EmployeeId = user.EmployeeId,

                    FirstName = user.FirstName,

                    LastName = user.LastName,

                    Email = user.Email,

                    PhoneNumber = user.PhoneNumber,

                    DesignationId = user.DesignationId,

                    DesignationName = user.Designation?.Name ?? string.Empty,

                    IsActive = user.IsActive,

                    DepartmentId = user.DepartmentId,

                    DepartmentName = user.Department.Name,

                    RoleName = roleName,

                    TotalBytes = wallets.TryGetValue(user.Id, out var bytes) ? bytes : 0
                });
        }

        return Result<List<UserResponse>>
            .Ok(response);
    }
}