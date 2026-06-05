using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Appreciations.Features.GetAppreciations;

namespace BytesRewards.Service.Appreciations.Features.GetAppreciationHistory;

public sealed record GetAppreciationHistoryQuery(
    Guid UserId)
    : IQuery<Result<List<AppreciationResponse>>>;