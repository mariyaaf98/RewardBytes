using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Wallets.Features.GetWalletLedger;

public sealed class GetWalletLedgerQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<GetWalletLedgerQuery, List<GetWalletLedgerResponse>>
{
    public async ValueTask<List<GetWalletLedgerResponse>> Handle(
        GetWalletLedgerQuery request,
        CancellationToken ct)
    {
        // ── Credits: bytes received from manager rewards ─────────
        var rewardCredits = await context.Rewards
            .Where(x => x.ToUserId == request.UserId)
            .Select(x => new GetWalletLedgerResponse
            {
                RewardId           = x.Id,
                RewardCategoryName = x.RewardCategoryName,
                Bytes              = x.Bytes,
                AwardedBy          = x.FromUserName,
                Reason             = x.Reason,
                AwardedAt          = x.CreatedAt,
                EntryType          = "Reward"
            })
            .ToListAsync(ct);

        // ── Refund credits: bytes returned when admin rejects ─────
        // A Rejected redemption means bytes were credited back to the wallet.
        var refundCredits = await context.Redemptions
            .Where(x => x.UserId == request.UserId && x.Status == "Rejected")
            .Select(x => new GetWalletLedgerResponse
            {
                RewardId           = x.Id,
                RewardCategoryName = "Redemption Refund",
                Bytes              = x.RedeemedBytes,
                AwardedBy          = "System",
                Reason             = $"Refund for rejected redemption: {x.ProductName}",
                AwardedAt          = x.UpdatedAt ?? x.CreatedAt,
                EntryType          = "Refund"
            })
            .ToListAsync(ct);

        // Merge and sort newest first
        return [.. rewardCredits
            .Concat(refundCredits)
            .OrderByDescending(x => x.AwardedAt)];
    }
}
