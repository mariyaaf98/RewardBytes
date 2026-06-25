using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Designations.Features.DeleteDesignation;

public sealed record DeleteDesignationCommand(Guid Id) : ICommand<Result<bool>>;
