using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Rewards.Domain;
using BytesRewards.Service.Wallets.Domain;
using BytesRewards.Service.Notifications.Services;

namespace BytesRewards.Service.Rewards.Features.CreateReward;

public sealed class CreateRewardCommandHandler(
    ApplicationDbContext context,
    NotificationService notifications)
    : ICommandHandler<CreateRewardCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateRewardCommand request,
        CancellationToken ct)
    {
        // Snapshot category, sender, recipient names
        var category = await context.RewardCategories
            .Where(x => x.Id == request.RewardCategoryId)
            .Select(x => new { x.Bytes, x.Name })
            .FirstOrDefaultAsync(ct);

        var fromUser = await context.Users
            .Where(x => x.Id == request.FromUserId)
            .Select(x => new { FullName = x.FirstName + " " + x.LastName })
            .FirstOrDefaultAsync(ct);

        var toUser = await context.Users
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

        // ── Wallet ──────────────────────────────────────────────
        var wallet = await context.Wallets
            .FirstOrDefaultAsync(x => x.UserId == request.ToUserId, ct);

        if (wallet is null)
        {
            context.Wallets.Add(new Wallet
            {
                Id             = Guid.NewGuid(),
                UserId         = request.ToUserId,
                AvailableBytes = reward.Bytes,
                CreatedAt      = DateTime.UtcNow
            });
        }
        else
        {
            wallet.AvailableBytes += reward.Bytes;
        }

        // ── Notifications ───────────────────────────────────────
        notifications.Create(
            userId:  request.ToUserId,
            type:    "RewardReceived",
            title:   $"🏅 You received a reward!",
            message: $"{reward.FromUserName} awarded you {reward.Bytes} bytes " +
                     $"for \"{reward.RewardCategoryName}\". Reason: {reward.Reason}");

        notifications.Create(
            userId:  request.FromUserId,
            type:    "RewardSent",
            title:   $"✅ Reward assigned to {reward.ToUserName}",
            message: $"You awarded {reward.Bytes} bytes ({reward.RewardCategoryName}) " +
                     $"to {reward.ToUserName}. Reason: {reward.Reason}");

        await notifications.CreateForAllUsersExceptAsync(
            excludeUserIds: [request.FromUserId, request.ToUserId],
            type:           "TeamRecognition",
            title:          $"🎉 {reward.ToUserName} was recognised!",
            message:        $"{reward.FromUserName} awarded {reward.Bytes} bytes " +
                            $"({reward.RewardCategoryName}) to {reward.ToUserName}.",
            ct:             ct);

        // ── Single atomic save — reward + wallet + notifications ─
        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(reward.Id);
    }
}
