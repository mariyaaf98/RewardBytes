using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Appreciations.Features.GetAppreciations;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciationById;

public sealed record GetAppreciationByIdQuery(
    Guid Id)
    : IQuery<Result<AppreciationResponse>>;