using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.RewardsCatalog.Features.GetRewardItems;

public sealed record GetRewardItemsQuery()
    : IQuery<List<RewardItemResponse>>;