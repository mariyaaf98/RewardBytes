using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Redemptions.Features.RedeemReward;

public sealed record RedeemRewardCommand(
    Guid UserId,
    Guid RewardItemId
)
    : ICommand<Result<Guid>>;