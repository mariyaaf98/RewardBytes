using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Rewards.Features.GetRewardHistory;

public sealed class GetRewardHistoryEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetRewardHistoryRequest,
        List<RewardHistoryResponse>>
{
    public override void Configure()
    {
        Get("/rewards/history/{userId}");

        Roles(
            "employee",
            "manager",
            "admin");

        Options(option =>
            option.WithTags("06 - Rewards"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<List<RewardHistoryResponse>>>
        ExecuteAsync(
            GetRewardHistoryRequest req,
            CancellationToken ct)
    {
        var rewards =
            await mediator.Send(
                new GetRewardHistoryQuery(
                    req.UserId),
                ct);

        return Result<List<RewardHistoryResponse>>
            .Ok(rewards);
    }
}