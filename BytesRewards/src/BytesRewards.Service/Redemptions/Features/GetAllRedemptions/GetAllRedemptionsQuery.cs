using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Redemptions.Features.GetAllRedemptions;

public sealed record GetAllRedemptionsQuery
    : IQuery<List<GetAllRedemptionsResponse>>;
