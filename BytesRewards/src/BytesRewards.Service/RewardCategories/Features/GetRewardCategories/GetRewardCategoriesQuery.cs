using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.RewardCategories.Features.GetRewardCategories;

public sealed record GetRewardCategoriesQuery
    : IQuery<List<RewardCategoryResponse>>;