using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Designations.Features.CreateDesignation;

public sealed class CreateDesignationEndpoint(IMediator mediator)
    : SecureFastEndpoint<CreateDesignationRequest, Guid>
{
    public override void Configure()
    {
        Post("/designations");
        Roles("admin");
        Options(o => o.WithTags("12 - Designations"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy() => new()
    {
        SecurityLevel = SecurityLevel.Internal,
        CachePolicy   = CachePolicy.NoStore
    };

    protected override async Task<Result<Guid>> ExecuteAsync(
        CreateDesignationRequest req, CancellationToken ct)
        => await mediator.Send(new CreateDesignationCommand(req.Name, req.Description), ct);
}
