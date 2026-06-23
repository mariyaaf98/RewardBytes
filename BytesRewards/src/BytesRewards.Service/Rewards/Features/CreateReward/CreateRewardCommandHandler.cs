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
        // Snapshot both bytes AND category name at creation time
        // so future admin edits to the category never change history.
        var category =
            await context.RewardCategories
                .Where(x => x.Id == request.RewardCategoryId)
                .Select(x => new { x.Bytes, x.Name })
                .FirstOrDefaultAsync(ct);

        var fromUser =
            await context.Users
                .Where(x => x.Id == request.FromUserId)
                .Select(x => new { FullName = x.FirstName + " " + x.LastName })
                .FirstOrDefaultAsync(ct);

        var toUser =
            await context.Users
                .Where(x => x.Id == request.ToUserId)
                .Select(x => new { FullName = x.FirstName + " " + x.LastName })
                .FirstOrDefaultAsync(ct);

        var reward = new Reward
        {
            Id                 = Guid.NewGuid(),
            FromUserId         = request.FromUserId,
            ToUserId           = request.ToUserId,
            RewardCategoryId   = request.RewardCategoryId,
            Reason             = request.Reason,
            Bytes              = category?.Bytes ?? 0,
            RewardCategoryName = category?.Name  ?? string.Empty,
            FromUserName       = fromUser?.FullName ?? string.Empty,
            ToUserName         = toUser?.FullName   ?? string.Empty,
            CreatedAt          = DateTime.UtcNow
        };

        context.Rewards.Add(reward);
        await context.SaveChangesAsync(ct);

        var wallet =
            await context.Wallets
                .FirstOrDefaultAsync(
                    x => x.UserId == request.ToUserId,
                    ct);

        if (wallet is null)
        {
            wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = request.ToUserId,
                AvailableBytes = reward.Bytes,
                CreatedAt = DateTime.UtcNow
            };
            context.Wallets.Add(wallet);
        }
        else
        {
            wallet.AvailableBytes += reward.Bytes;
        }

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(reward.Id);
    }
}