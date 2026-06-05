using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

using BytesRewards.Service.Appreciations.Features.GetAppreciations;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciationHistory;

public sealed class GetAppreciationHistoryEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetAppreciationHistoryRequest,
        List<AppreciationResponse>>
{
    public override void Configure()
    {
        Get("/users/{userId}/appreciation-history");

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

    protected override async Task<Result<List<AppreciationResponse>>>
        ExecuteAsync(
            GetAppreciationHistoryRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new GetAppreciationHistoryQuery(
                req.UserId),
            ct);
    }
}