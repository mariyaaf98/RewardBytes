using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Rewards.Features.GetRewardHistory;

public sealed class GetRewardHistoryQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetRewardHistoryQuery,
        List<RewardHistoryResponse>>
{
    public async ValueTask<List<RewardHistoryResponse>> Handle(
        GetRewardHistoryQuery request,
        CancellationToken ct)
    {
        return await context.Rewards
            .Where(x =>
                x.ToUserId == request.UserId)
            .Select(x => new RewardHistoryResponse
            {
                RewardId = x.Id,

                RewardCategoryName = x.RewardCategoryName,

                Bytes = x.Bytes,

                AwardedBy = x.FromUserName,

                Reason = x.Reason,

                AwardedAt = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}