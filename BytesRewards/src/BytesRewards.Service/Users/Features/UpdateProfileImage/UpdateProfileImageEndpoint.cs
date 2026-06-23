using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;
using System.Security.Claims;

namespace BytesRewards.Service.Users.Features.UpdateProfileImage;

public sealed class UpdateProfileImageEndpoint(IMediator mediator)
    : SecureFastEndpoint<UpdateProfileImageRequest, bool>
{
    public override void Configure()
    {
        Put("/users/me/profile-image");
        Roles("employee", "manager", "admin");
        Options(o => o.WithTags("01 - Users"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy   = CachePolicy.NoStore
    };

    protected override async Task<Result<bool>> ExecuteAsync(
        UpdateProfileImageRequest req,
        CancellationToken ct)
    {
        var keycloakUserId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(keycloakUserId))
            throw new Exception("Keycloak user id not found.");

        return await mediator.Send(
            new UpdateProfileImageCommand(keycloakUserId, req.ProfileImageUrl), ct);
    }
}

public sealed class UpdateProfileImageRequest
{
    public string ProfileImageUrl { get; set; } = string.Empty;
}
