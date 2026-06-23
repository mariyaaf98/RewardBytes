using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.Redemptions.Features.RedeemReward;

public sealed class RedeemRewardEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        RedeemRewardRequest,
        Guid>
{
    public override void Configure()
    {
        Post("/redemptions");

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

    protected override async Task<Result<Guid>>
        ExecuteAsync(
            RedeemRewardRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new RedeemRewardCommand(
                req.UserId,
                req.RewardItemId),
            ct);
    }
}