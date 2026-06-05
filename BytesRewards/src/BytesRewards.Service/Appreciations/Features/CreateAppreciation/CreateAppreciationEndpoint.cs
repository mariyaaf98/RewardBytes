using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Appreciations.Features.CreateAppreciation;

public sealed class CreateAppreciationEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<CreateAppreciationRequest, Guid>
{
    public override void Configure()
    {
        Post("/appreciations");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags("04 - Appreciations"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<Guid>> ExecuteAsync(
        CreateAppreciationRequest req,
        CancellationToken ct)
    {
        return await mediator.Send(
            new CreateAppreciationCommand(
                req.FromUserId,
                req.ToUserId,
                req.Message),
            ct);
    }
}