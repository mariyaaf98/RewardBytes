using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;
using System.Security.Claims;

namespace BytesRewards.Service.Users.Features.ChangePassword;

public sealed class ChangePasswordEndpoint(IMediator mediator)
    : SecureFastEndpoint<ChangePasswordRequest, bool>
{
    public override void Configure()
    {
        Put("/users/me/password");
        Roles("employee", "manager", "admin");
        Options(o => o.WithTags("01 - Users"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy = CachePolicy.NoStore
    };

    protected override async Task<Result<bool>> ExecuteAsync(
        ChangePasswordRequest req,
        CancellationToken ct)
    {
        var keycloakUserId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(keycloakUserId))
            throw new Exception("Keycloak user id not found.");

        return await mediator.Send(
    new ChangePasswordCommand(
        keycloakUserId,
        req.CurrentPassword,
        req.NewPassword),
    ct);
    }
}

public sealed class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty; // validated client-side only
    public string NewPassword { get; set; } = string.Empty;
}
