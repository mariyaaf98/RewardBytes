using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Redemptions.Features.GetRedemptionHistory;

public sealed class GetRedemptionHistoryEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetRedemptionHistoryRequest,
        List<GetRedemptionHistoryResponse>>
{
    public override void Configure()
    {
        Get("/redemptions/history/{userId}");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags(
                "10 - Redemptions"));
    }

    protected override SecurityCachePolicy
        GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel =
                SecurityLevel.Internal,

            CachePolicy =
                CachePolicy.NoStore
        };

    protected override async Task<Result<List<GetRedemptionHistoryResponse>>>
        ExecuteAsync(
            GetRedemptionHistoryRequest req,
            CancellationToken ct)
    {
        var result =
            await mediator.Send(
                new GetRedemptionHistoryQuery(
                    req.UserId),
                ct);

        return Result<List<GetRedemptionHistoryResponse>>
            .Ok(result);
    }
}