using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Redemptions.Features.GetRedemptionHistory;

public sealed record GetRedemptionHistoryQuery(
    Guid UserId)
    : IQuery<List<GetRedemptionHistoryResponse>>;