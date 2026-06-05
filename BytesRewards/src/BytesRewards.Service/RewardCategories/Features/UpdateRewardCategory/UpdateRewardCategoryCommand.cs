using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.RewardCategories.Features.UpdateRewardCategory;

public sealed record UpdateRewardCategoryCommand(
    Guid Id,
    string Name,
    string Description,
    int Bytes)
    : ICommand<Result<bool>>;