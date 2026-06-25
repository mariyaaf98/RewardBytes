using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Notifications.Services;

namespace BytesRewards.Service.Redemptions.Features.UpdateRedemptionStatus;

public sealed class UpdateRedemptionStatusCommandHandler(
    ApplicationDbContext context,
    NotificationService notifications)
    : ICommandHandler<UpdateRedemptionStatusCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        UpdateRedemptionStatusCommand request,
        CancellationToken ct)
    {
        var redemption = await context.Redemptions
            .FirstOrDefaultAsync(x => x.Id == request.RedemptionId, ct)
            ?? throw new Exception("Redemption not found.");

        if (redemption.Status == "Delivered" || redemption.Status == "Rejected")
            throw new Exception($"Cannot change status from '{redemption.Status}'.");

        var previousStatus = redemption.Status;
        redemption.Status  = request.Status;

        // ── Refund bytes when admin rejects ──────────────────────
        if (request.Status == "Rejected" && previousStatus != "Rejected")
        {
            var wallet = await context.Wallets
                .FirstOrDefaultAsync(x => x.UserId == redemption.UserId, ct);

            if (wallet is not null)
                wallet.AvailableBytes += redemption.RedeemedBytes;
        }

        // ── Notify employee ──────────────────────────────────────
        var (type, title, message) = request.Status switch
        {
            "Approved"  => ("RedemptionApproved",
                            $"✅ Redemption approved — {redemption.ProductName}",
                            $"Your redemption of \"{redemption.ProductName}\" has been approved. " +
                            $"It will be delivered soon."),

            "Rejected"  => ("RedemptionRejected",
                            $"❌ Redemption rejected — {redemption.ProductName}",
                            $"Your redemption of \"{redemption.ProductName}\" was rejected. " +
                            $"{redemption.RedeemedBytes} bytes have been refunded to your wallet."),

            "Delivered" => ("RedemptionDelivered",
                            $"📦 Reward delivered — {redemption.ProductName}",
                            $"Your reward \"{redemption.ProductName}\" has been delivered. Enjoy!"),

            _ => ("RedemptionUpdated",
                  $"🔔 Redemption status updated",
                  $"Your redemption status changed to {request.Status}.")
        };

        notifications.Create(redemption.UserId, type, title, message);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(redemption.Id);
    }
}
