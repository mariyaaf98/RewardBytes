using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Rewards.Features.GetRewardHistory;

public sealed record GetRewardHistoryQuery(
    Guid UserId)
    : IQuery<List<RewardHistoryResponse>>;