using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

using BytesRewards.Service.Appreciations.Features.GetAppreciations;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciationById;

public sealed class GetAppreciationByIdEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetAppreciationByIdRequest,
        AppreciationResponse>
{
    public override void Configure()
    {
        Get("/appreciations/{id}");

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

    protected override async Task<Result<AppreciationResponse>>
        ExecuteAsync(
            GetAppreciationByIdRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new GetAppreciationByIdQuery(req.Id),
            ct);
    }
}