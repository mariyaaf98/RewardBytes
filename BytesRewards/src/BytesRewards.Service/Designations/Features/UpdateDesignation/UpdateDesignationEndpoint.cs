using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Designations.Features.UpdateDesignation;

public sealed class UpdateDesignationEndpoint(IMediator mediator)
    : SecureFastEndpoint<UpdateDesignationRequest, bool>
{
    public override void Configure()
    {
        Put("/designations/{id}");
        Roles("admin");
        Options(o => o.WithTags("12 - Designations"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy   = CachePolicy.NoStore
    };

    protected override async Task<Result<bool>> ExecuteAsync(
        UpdateDesignationRequest req, CancellationToken ct)
        => await mediator.Send(
            new UpdateDesignationCommand(req.Id, req.Name, req.Description), ct);
}
