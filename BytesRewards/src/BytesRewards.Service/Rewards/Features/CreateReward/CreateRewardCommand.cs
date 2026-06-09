using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Rewards.Features.CreateReward;

public sealed record CreateRewardCommand(
    Guid FromUserId,
    Guid ToUserId,
    Guid RewardCategoryId,
    string Reason)
    : ICommand<Result<Guid>>;