using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.RewardsCatalog.Features.CreateRewardItem;

public sealed record CreateRewardItemCommand(
    string ProductCode,
    string Name,
    string Description,
    int RequiredBytes,
    bool IsActive,
    string ImageUrl
)
    : ICommand<Result<Guid>>;