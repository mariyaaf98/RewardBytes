using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure.Security.Keycloak;
using Microsoft.EntityFrameworkCore;
using BytesRewards.Service.Infrastructure;


namespace BytesRewards.Service.Users.Features.ChangePassword;

/// <summary>
/// The user is already authenticated via a valid Keycloak JWT (verified by the endpoint
/// guard). We trust the active session as proof of identity and skip re-verifying the
/// current password against Keycloak — the "bytes-rewards-api" client does not have
/// Direct Access Grants enabled which would make a password grant return 400 BadRequest.
///
/// The current password field is still collected on the frontend as a UX safeguard
/// against accidental changes on a shared/unlocked screen.
/// </summary>
public sealed class ChangePasswordCommandHandler(
    IKeycloakAdminService keycloakService,
    ApplicationDbContext _context)
    : ICommandHandler<ChangePasswordCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        ChangePasswordCommand request,
        CancellationToken ct)
    {

       var user = await _context.Users
    .FirstOrDefaultAsync(
        x => x.KeycloakUserId == request.KeycloakUserId,
        ct);

        if (user is null)
        {
            throw new Exception("User not found.");
        }

        var isValid =
            await keycloakService.ValidatePasswordAsync(
                user.Email,
                request.CurrentPassword,
                ct);

        if (!isValid)
        {
            throw new Exception("Current password is incorrect.");
        }

        var adminToken =
            await keycloakService.GetAdminTokenAsync(ct);

        await keycloakService.ResetPasswordAsync(
            adminToken,
            request.KeycloakUserId,
            request.NewPassword,
            ct);

        return Result<bool>.Ok(true);
    }
}
