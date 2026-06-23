using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Redemptions.Domain;

namespace BytesRewards.Service.Redemptions.Features.RedeemReward;

public sealed class RedeemRewardCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        RedeemRewardCommand,
        Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        RedeemRewardCommand request,
        CancellationToken ct)
    {
        var wallet =
            await context.Wallets
                .FirstOrDefaultAsync(
                    x => x.UserId == request.UserId,
                    ct);

        if (wallet is null)
        {
            throw new Exception(
                "Wallet not found");
        }

        var rewardItem =
            await context.RewardItems
                .FirstOrDefaultAsync(
                    x => x.Id == request.RewardItemId
                      && x.IsActive,
                    ct);

        if (rewardItem is null)
        {
            throw new Exception(
                "Reward item not found");
        }

        if (wallet.AvailableBytes <
            rewardItem.RequiredBytes)
        {
            throw new Exception(
                "Insufficient bytes");
        }

        wallet.AvailableBytes -=
            rewardItem.RequiredBytes;

        var redemption = new Redemption
        {
            Id             = Guid.NewGuid(),
            UserId         = request.UserId,
            RewardItemId   = rewardItem.Id,
            RedeemedBytes  = rewardItem.RequiredBytes,
            ProductName    = rewardItem.Name,       // snapshot
            Status         = "Pending",
            CreatedAt      = DateTime.UtcNow
        };

        context.Redemptions.Add(
            redemption);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(
            redemption.Id);
    }
}