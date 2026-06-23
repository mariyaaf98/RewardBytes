using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.RewardsCatalog.Features.UpdateRewardItem;

public sealed record UpdateRewardItemCommand(
    Guid Id,
    string ProductCode,
    string Name,
    string Description,
    int RequiredBytes,
    bool IsActive,
    string ImageUrl
)
    : ICommand<Result<Guid>>;