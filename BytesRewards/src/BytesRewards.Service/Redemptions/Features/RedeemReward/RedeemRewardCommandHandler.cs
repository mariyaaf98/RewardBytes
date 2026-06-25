using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Notifications.Services;
using BytesRewards.Service.Redemptions.Domain;

namespace BytesRewards.Service.Redemptions.Features.RedeemReward;

public sealed class RedeemRewardCommandHandler(
    ApplicationDbContext context,
    NotificationService notifications)
    : ICommandHandler<RedeemRewardCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        RedeemRewardCommand request,
        CancellationToken ct)
    {
        var wallet = await context.Wallets
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, ct)
            ?? throw new Exception("Wallet not found");

        var rewardItem = await context.RewardItems
            .FirstOrDefaultAsync(x => x.Id == request.RewardItemId && x.IsActive, ct)
            ?? throw new Exception("Reward item not found");

        if (wallet.AvailableBytes < rewardItem.RequiredBytes)
            throw new Exception("Insufficient bytes");

        wallet.AvailableBytes -= rewardItem.RequiredBytes;

        var redemption = new Redemption
        {
            Id            = Guid.NewGuid(),
            UserId        = request.UserId,
            RewardItemId  = rewardItem.Id,
            RedeemedBytes = rewardItem.RequiredBytes,
            ProductName   = rewardItem.Name,
            Status        = "Pending",
            CreatedAt     = DateTime.UtcNow
        };

        context.Redemptions.Add(redemption);

        // ── Notify employee ──────────────────────────────────────
        notifications.Create(
            userId:  request.UserId,
            type:    "RedemptionPending",
            title:   $"🛒 Redemption submitted — {rewardItem.Name}",
            message: $"Your redemption request for \"{rewardItem.Name}\" " +
                     $"({rewardItem.RequiredBytes} bytes) is pending admin approval.");

        // ── Notify all admins — broadcast to all except the employee ──
        var employee = await context.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new { FullName = u.FirstName + " " + u.LastName })
            .FirstOrDefaultAsync(ct);

        await notifications.CreateForAllUsersExceptAsync(
            excludeUserIds: [request.UserId],
            type:           "NewRedemptionRequest",
            title:          $"🛒 New redemption request",
            message:        $"{employee?.FullName ?? "An employee"} requested \"{rewardItem.Name}\" " +
                            $"({rewardItem.RequiredBytes} bytes). Pending your approval.",
            ct:             ct);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(redemption.Id);
    }
}
