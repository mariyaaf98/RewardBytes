using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure.Security.Keycloak;
using Microsoft.EntityFrameworkCore;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Users.Features.ChangePassword;

/// <summary>
/// The user is already authenticated via a valid Keycloak JWT (verified by the
/// endpoint guard). The active session is sufficient proof of identity.
///
/// ValidatePasswordAsync uses the backend service-account client
/// ("bytes-rewards-api") which has Client Credentials grant, NOT Direct Access
/// Grants — so it cannot validate a user password via the token endpoint.
///
/// The current password field is enforced on the frontend only (client-side UX
/// guard against accidental changes). The backend trusts the JWT and resets
/// the password directly via the Keycloak Admin API.
/// </summary>
public sealed class ChangePasswordCommandHandler(
    IKeycloakAdminService keycloakService,
    ApplicationDbContext context)
    : ICommandHandler<ChangePasswordCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        ChangePasswordCommand request,
        CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(
                x => x.KeycloakUserId == request.KeycloakUserId, ct)
            ?? throw new Exception("User not found.");

        // JWT already proves identity — reset password directly.
        var adminToken = await keycloakService.GetAdminTokenAsync(ct);

        await keycloakService.ResetPasswordAsync(
            adminToken,
            request.KeycloakUserId,
            request.NewPassword,
            ct);

        return Result<bool>.Ok(true);
    }
}
