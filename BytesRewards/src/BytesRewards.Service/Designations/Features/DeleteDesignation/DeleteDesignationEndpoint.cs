using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Designations.Features.DeleteDesignation;

public sealed class DeleteDesignationEndpoint(IMediator mediator)
    : SecureFastEndpoint<DeleteDesignationRequest, bool>
{
    public override void Configure()
    {
        Delete("/designations/{id}");
        Roles("admin");
        Options(o => o.WithTags("12 - Designations"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy   = CachePolicy.NoStore
    };

    protected override async Task<Result<bool>> ExecuteAsync(
        DeleteDesignationRequest req, CancellationToken ct)
        => await mediator.Send(new DeleteDesignationCommand(req.Id), ct);
}
