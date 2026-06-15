using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Users.Features.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetCurrentUserQuery,
        GetCurrentUserResponse>
{
    public async ValueTask<GetCurrentUserResponse> Handle(
        GetCurrentUserQuery request,
        CancellationToken ct)
    {
        var user =
            await context.Users
                .Include(x => x.Department)
                .FirstOrDefaultAsync(
                    x => x.KeycloakUserId == request.KeycloakUserId,
                    ct);

        if (user is null)
        {
            throw new Exception(
                "Current user not found.");
        }

        return new GetCurrentUserResponse
        {
            Id              = user.Id,
            EmployeeId      = user.EmployeeId,
            FirstName       = user.FirstName,
            LastName        = user.LastName,
            Email           = user.Email,
            PhoneNumber     = user.PhoneNumber,
            Designation     = user.Designation,
            ProfileImageUrl = user.ProfileImageUrl,
            IsActive        = user.IsActive,
            DepartmentId    = user.DepartmentId.ToString(),
            DepartmentName  = user.Department?.Name ?? string.Empty,
            RoleName        = string.Empty // resolved from Keycloak token on frontend
        };
    }
}
