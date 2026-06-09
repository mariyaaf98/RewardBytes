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

                RewardCategoryName =
                    context.RewardCategories
                        .Where(c =>
                            c.Id == x.RewardCategoryId)
                        .Select(c => c.Name)
                        .FirstOrDefault() ?? string.Empty,

                Bytes =
                    context.RewardCategories
                        .Where(c =>
                            c.Id == x.RewardCategoryId)
                        .Select(c => c.Bytes)
                        .FirstOrDefault(),

                AwardedBy =
                    context.Users
                        .Where(u =>
                            u.Id == x.FromUserId)
                        .Select(u =>
                            u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? string.Empty,

                Reason = x.Reason,

                AwardedAt = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}