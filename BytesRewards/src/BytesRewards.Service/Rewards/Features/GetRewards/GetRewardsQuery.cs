using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Rewards.Features.GetRewards;

public sealed record GetRewardsQuery
    : IQuery<List<RewardResponse>>;