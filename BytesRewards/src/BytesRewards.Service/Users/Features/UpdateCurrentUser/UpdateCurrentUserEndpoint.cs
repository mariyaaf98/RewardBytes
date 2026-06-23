using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;
using System.Security.Claims;

namespace BytesRewards.Service.Users.Features.UpdateCurrentUser;

/// <summary>
/// PUT /users/me
/// Allows any authenticated employee/manager/admin to update
/// their own firstName, lastName, and phoneNumber.
/// Role and department changes are admin-only via PUT /users/{id}.
/// </summary>
public sealed class UpdateCurrentUserEndpoint(IMediator mediator)
    : SecureFastEndpoint<UpdateCurrentUserRequest, bool>
{
    public override void Configure()
    {
        Put("/users/me");
        Roles("employee", "manager", "admin");
        Options(o => o.WithTags("01 - Users"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy   = CachePolicy.NoStore
    };

    protected override async Task<Result<bool>> ExecuteAsync(
        UpdateCurrentUserRequest req,
        CancellationToken ct)
    {
        var keycloakUserId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(keycloakUserId))
            throw new Exception("Keycloak user id not found.");

        return await mediator.Send(
            new UpdateCurrentUserCommand(
                keycloakUserId,
                req.FirstName,
                req.LastName,
                req.PhoneNumber),
            ct);
    }
}

public sealed class UpdateCurrentUserRequest
{
    public string FirstName   { get; set; } = string.Empty;
    public string LastName    { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
