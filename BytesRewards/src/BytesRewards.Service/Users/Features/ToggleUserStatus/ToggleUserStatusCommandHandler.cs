using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Infrastructure.Security.Keycloak;

using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Users.Features.ToggleUserStatus;

public sealed class ToggleUserStatusCommandHandler(
    ApplicationDbContext context,
    IKeycloakAdminService keycloakAdminService)
    : ICommandHandler<ToggleUserStatusCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        ToggleUserStatusCommand request,
        CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId, ct);

        if (user is null)
            return Result<bool>.Failure(
                new Error(
                    "user.not_found",
                    "User not found.",
                    404,
                    "users"));

        var token = await keycloakAdminService.GetAdminTokenAsync(ct);

        // Toggle: if currently active → disable; if inactive → enable
        if (user.IsActive)
        {
            await keycloakAdminService.DisableUserAsync(
                token, user.KeycloakUserId, ct);

            user.IsActive = false;
        }
        else
        {
            await keycloakAdminService.EnableUserAsync(
                token, user.KeycloakUserId, ct);

            user.IsActive = true;
        }

        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
