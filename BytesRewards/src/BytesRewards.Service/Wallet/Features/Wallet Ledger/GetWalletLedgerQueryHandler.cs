using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Wallets.Features.GetWalletLedger;

public sealed class GetWalletLedgerQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetWalletLedgerQuery,
        List<GetWalletLedgerResponse>>
{
    public async ValueTask<List<GetWalletLedgerResponse>> Handle(
        GetWalletLedgerQuery request,
        CancellationToken ct)
    {
        return await context.Rewards
            .Where(x =>
                x.ToUserId == request.UserId)
            .Select(x => new GetWalletLedgerResponse
            {
                RewardId = x.Id,

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

                AwardedBy =
                    context.Users
                        .Where(u =>
                            u.Id == x.FromUserId)
                        .Select(u =>
                            u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? string.Empty,

                Reason = x.Reason,

                AwardedAt = x.CreatedAt
            })
            .OrderByDescending(x =>
                x.AwardedAt)
            .ToListAsync(ct);
    }
}