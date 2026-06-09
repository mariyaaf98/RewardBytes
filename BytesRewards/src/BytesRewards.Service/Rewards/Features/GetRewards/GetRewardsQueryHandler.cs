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

                FromUserName =
                    context.Users
                        .Where(u => u.Id == x.FromUserId)
                        .Select(u =>
                            u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? string.Empty,

                ToUserName =
                    context.Users
                        .Where(u => u.Id == x.ToUserId)
                        .Select(u =>
                            u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? string.Empty,

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

                Reason = x.Reason,

                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}