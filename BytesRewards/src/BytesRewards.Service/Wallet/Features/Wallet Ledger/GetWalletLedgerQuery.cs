using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Wallets.Features.GetWalletLedger;

public sealed record GetWalletLedgerQuery(
    Guid UserId)
    : IQuery<List<GetWalletLedgerResponse>>;