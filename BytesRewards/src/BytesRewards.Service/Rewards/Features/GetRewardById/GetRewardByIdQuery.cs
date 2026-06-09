using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Rewards.Features.GetRewards;

namespace BytesRewards.Service.Rewards.Features.GetRewardById;

public sealed record GetRewardByIdQuery(
    Guid Id)
    : IQuery<RewardResponse>;