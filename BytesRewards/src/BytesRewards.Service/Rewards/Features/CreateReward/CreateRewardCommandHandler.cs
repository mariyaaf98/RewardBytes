using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Rewards.Domain;
using BytesRewards.Service.Wallets.Domain;

namespace BytesRewards.Service.Rewards.Features.CreateReward;

public sealed class CreateRewardCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        CreateRewardCommand,
        Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateRewardCommand request,
        CancellationToken ct)
    {
        var reward = new Reward
        {
            Id = Guid.NewGuid(),

            FromUserId = request.FromUserId,

            ToUserId = request.ToUserId,

            RewardCategoryId = request.RewardCategoryId,

            Reason = request.Reason,

            CreatedAt = DateTime.UtcNow
        };

        context.Rewards.Add(reward);

        await context.SaveChangesAsync(ct);

        var wallet =
            await context.Wallets
                .FirstOrDefaultAsync(
                    x => x.UserId == request.ToUserId,
                    ct);

        var rewardBytes =
    await context.RewardCategories
        .Where(x =>
            x.Id == request.RewardCategoryId)
        .Select(x => x.Bytes)
        .FirstOrDefaultAsync(ct);

        if (wallet is null)
        {
            wallet = new Wallet
            {
                Id = Guid.NewGuid(),

                UserId = request.ToUserId,

                AvailableBytes = rewardBytes,

                CreatedAt = DateTime.UtcNow
            };

            context.Wallets.Add(wallet);
        }
        else
        {
            wallet.AvailableBytes += rewardBytes;
        }

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(reward.Id);
    }
}