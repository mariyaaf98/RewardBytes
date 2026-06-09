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

            FromUserName =
                context.Users
                    .Where(x =>
                        x.Id == reward.FromUserId)
                    .Select(x =>
                        x.FirstName + " " + x.LastName)
                    .FirstOrDefault() ?? string.Empty,

            ToUserName =
                context.Users
                    .Where(x =>
                        x.Id == reward.ToUserId)
                    .Select(x =>
                        x.FirstName + " " + x.LastName)
                    .FirstOrDefault() ?? string.Empty,

            RewardCategoryName =
                context.RewardCategories
                    .Where(x =>
                        x.Id == reward.RewardCategoryId)
                    .Select(x => x.Name)
                    .FirstOrDefault() ?? string.Empty,

            Bytes =
                context.RewardCategories
                    .Where(x =>
                        x.Id == reward.RewardCategoryId)
                    .Select(x => x.Bytes)
                    .FirstOrDefault(),

            Reason = reward.Reason,

            CreatedAt = reward.CreatedAt
        };
    }
}