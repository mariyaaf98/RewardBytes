using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Redemptions.Features.GetRedemptionById;

public sealed record GetRedemptionByIdQuery(
    Guid Id)
    : IQuery<GetRedemptionByIdResponse>;