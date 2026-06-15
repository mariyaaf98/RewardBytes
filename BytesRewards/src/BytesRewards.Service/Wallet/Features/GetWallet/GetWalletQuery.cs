using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Wallets.Features.GetWallet;

public sealed record GetWalletQuery(
    Guid UserId)
    : IQuery<GetWalletResponse>;