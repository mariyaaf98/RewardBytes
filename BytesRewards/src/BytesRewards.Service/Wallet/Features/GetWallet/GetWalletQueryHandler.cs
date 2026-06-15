using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Wallets.Domain;

namespace BytesRewards.Service.Wallets.Features.GetWallet;

public sealed class GetWalletQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetWalletQuery,
        GetWalletResponse>
{
    public async ValueTask<GetWalletResponse> Handle(
        GetWalletQuery request,
        CancellationToken ct)
    {
        var wallet =
            await context.Wallets
                .FirstOrDefaultAsync(
                    x => x.UserId == request.UserId,
                    ct);

        if (wallet is null)
        {
            wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                AvailableBytes = 0,
                CreatedAt = DateTime.UtcNow
            };

            context.Wallets.Add(wallet);

            await context.SaveChangesAsync(ct);
        }

        return new GetWalletResponse
        {
            AvailableBytes =
                wallet.AvailableBytes
        };
    }
}