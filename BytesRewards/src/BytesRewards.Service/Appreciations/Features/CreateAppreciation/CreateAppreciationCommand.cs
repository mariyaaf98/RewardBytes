using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Appreciations.Features.CreateAppreciation;

public sealed record CreateAppreciationCommand(
    Guid FromUserId,
    Guid ToUserId,
    string Message)
    : ICommand<Result<Guid>>;