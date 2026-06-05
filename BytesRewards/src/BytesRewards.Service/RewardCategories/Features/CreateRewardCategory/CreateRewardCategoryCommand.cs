using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.RewardCategories.Features.CreateRewardCategory;

public sealed record CreateRewardCategoryCommand(
    string Name,
    string Description,
    int Bytes)
    : ICommand<Result<Guid>>;