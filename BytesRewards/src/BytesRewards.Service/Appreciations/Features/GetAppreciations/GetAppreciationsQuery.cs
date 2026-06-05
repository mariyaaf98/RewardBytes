using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciations;

public sealed record GetAppreciationsQuery
    : IQuery<List<AppreciationResponse>>;