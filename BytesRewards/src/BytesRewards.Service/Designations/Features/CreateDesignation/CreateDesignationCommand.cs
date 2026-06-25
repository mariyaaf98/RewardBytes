using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Designations.Features.CreateDesignation;

public sealed record CreateDesignationCommand(string Name, string Description)
    : ICommand<Result<Guid>>;
