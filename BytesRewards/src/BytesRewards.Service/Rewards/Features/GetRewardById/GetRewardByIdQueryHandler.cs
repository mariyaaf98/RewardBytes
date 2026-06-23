using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Rewards.Features.GetRewards;

namespace BytesRewards.Service.Rewards.Features.GetRewardById;

public sealed class GetRewardByIdQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetRewardByIdQuery,
        RewardResponse>
{
    public async ValueTask<RewardResponse> Handle(
        GetRewardByIdQuery request,
        CancellationToken ct)
    {
        var reward =
            await context.Rewards
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    ct);

        if (reward is null)
        {
            throw new Exception(
                "Reward not found");
        }

        return new RewardResponse
        {
            Id = reward.Id,

            FromUserName       = reward.FromUserName,
            ToUserName         = reward.ToUserName,
            RewardCategoryName = reward.RewardCategoryName,
            Bytes              = reward.Bytes,

            Reason = reward.Reason,

            CreatedAt = reward.CreatedAt
        };
    }
}