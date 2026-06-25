using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Infrastructure.Security.Keycloak;
using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Users.Features.UpdateUser;

public sealed class UpdateUserCommandHandler(
    ApplicationDbContext context,
    IKeycloakAdminService keycloakAdminService
)
: ICommandHandler<UpdateUserCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        UpdateUserCommand request,
        CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                ct);

        if (user == null)
        {
            return Result<bool>.Ok(false);
        }

        user.FirstName = request.FirstName;

        user.LastName = request.LastName;

        user.PhoneNumber = request.PhoneNumber;

        user.DesignationId = request.DesignationId;

        user.DepartmentId = request.DepartmentId;

        user.Email = request.Email;
        
        user.UpdatedAt = DateTime.UtcNow;

        var token =
            await keycloakAdminService
                .GetAdminTokenAsync(ct);

        await keycloakAdminService.UpdateUserAsync(
            token,
            user.KeycloakUserId,
            request.FirstName,
            request.LastName,
            request.Email,
            ct);

        await keycloakAdminService.UpdateUserRoleAsync(
            token,
            user.KeycloakUserId,
            request.Role,
            ct);



        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}