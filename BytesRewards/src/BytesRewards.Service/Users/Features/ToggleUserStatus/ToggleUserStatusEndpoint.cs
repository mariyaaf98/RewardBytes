using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Users.Features.ToggleUserStatus;

public sealed class ToggleUserStatusEndpoint(IMediator mediator)
    : SecureFastEndpoint<ToggleUserStatusRequest, bool>
{
    public override void Configure()
    {
        Patch("/users/{id}/toggle-status");

        Roles("admin");

        Summary(s => s.Summary = "Block or unblock a user");

        Options(o => o.WithTags("01 - Users"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy = CachePolicy.NoStore
    };

    protected override async Task<Result<bool>> ExecuteAsync(
        ToggleUserStatusRequest req,
        CancellationToken ct)
    {
        return await mediator.Send(
            new ToggleUserStatusCommand(req.Id),
            ct);
    }
}
