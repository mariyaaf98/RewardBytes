using AppWeaver.Mediator.Interfaces;

using AppWeaver.Results;

namespace BytesRewards.Service.Users.Features.DeleteUser;

public sealed record DeleteUserCommand(Guid Id)
    : ICommand<Result<bool>>;