using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Redemptions.Features.UpdateRedemptionStatus;

public sealed class UpdateRedemptionStatusCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        UpdateRedemptionStatusCommand,
        Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        UpdateRedemptionStatusCommand request,
        CancellationToken ct)
    {
        var redemption =
            await context.Redemptions
                .FirstOrDefaultAsync(
                    x => x.Id == request.RedemptionId,
                    ct);

        if (redemption is null)
            throw new Exception("Redemption not found.");

        // Prevent invalid transitions
        if (redemption.Status == "Delivered" || redemption.Status == "Rejected")
            throw new Exception(
                $"Cannot change status from '{redemption.Status}'.");

        var previousStatus = redemption.Status;
        redemption.Status  = request.Status;

        // ── Refund bytes when admin rejects ──────────────────────
        // Bytes were deducted at redemption time (Pending).
        // If admin rejects, credit them back to the employee's wallet.
        if (request.Status == "Rejected" && previousStatus != "Rejected")
        {
            var wallet =
                await context.Wallets
                    .FirstOrDefaultAsync(
                        x => x.UserId == redemption.UserId,
                        ct);

            if (wallet is not null)
            {
                wallet.AvailableBytes += redemption.RedeemedBytes;
            }
        }

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(redemption.Id);
    }
}
