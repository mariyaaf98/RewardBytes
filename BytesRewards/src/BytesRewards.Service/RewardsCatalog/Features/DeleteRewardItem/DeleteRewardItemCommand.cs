using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.RewardsCatalog.Features.DeleteRewardItem;

public sealed record DeleteRewardItemCommand(
    Guid Id)
    : ICommand<Result<Guid>>;