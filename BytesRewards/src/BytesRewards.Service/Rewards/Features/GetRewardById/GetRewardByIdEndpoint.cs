using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

using BytesRewards.Service.Rewards.Features.GetRewards;

namespace BytesRewards.Service.Rewards.Features.GetRewardById;

public sealed class GetRewardByIdEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        GetRewardByIdRequest,
        RewardResponse>
{
    public override void Configure()
    {
        Get("/rewards/{id}");

        Roles(
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

    protected override async Task<Result<RewardResponse>>
    ExecuteAsync(
        GetRewardByIdRequest req,
        CancellationToken ct)
    {
        var reward = await mediator.Send(
            new GetRewardByIdQuery(req.Id),
            ct);

        return Result<RewardResponse>.Ok(reward);
    }
}