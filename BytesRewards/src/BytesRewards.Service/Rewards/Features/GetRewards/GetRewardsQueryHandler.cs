using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Rewards.Features.GetRewards;

public sealed class GetRewardsQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetRewardsQuery,
        List<RewardResponse>>
{
    public async ValueTask<List<RewardResponse>> Handle(
        GetRewardsQuery request,
        CancellationToken ct)
    {
        return await context.Rewards
            .Select(x => new RewardResponse
            {
                Id = x.Id,

                FromUserName       = x.FromUserName,
                ToUserName         = x.ToUserName,
                RewardCategoryName = x.RewardCategoryName,
                Bytes              = x.Bytes,

                Reason = x.Reason,

                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}