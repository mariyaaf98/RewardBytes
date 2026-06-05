using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.RewardCategories.Features.DeleteRewardCategory;

public sealed record DeleteRewardCategoryCommand(
    Guid Id)
    : ICommand<Result<bool>>;