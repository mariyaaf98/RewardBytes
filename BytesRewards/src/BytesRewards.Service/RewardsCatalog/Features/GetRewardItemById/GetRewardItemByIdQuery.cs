using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.RewardsCatalog.Features.GetRewardItemById;

public sealed record GetRewardItemByIdQuery(
    Guid Id)
    : IQuery<GetRewardItemByIdResponse>;