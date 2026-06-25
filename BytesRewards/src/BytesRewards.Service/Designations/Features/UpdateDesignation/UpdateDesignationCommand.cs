using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Designations.Features.UpdateDesignation;

public sealed record UpdateDesignationCommand(
    Guid Id,
    string Name,
    string Description
) : ICommand<Result<bool>>;
