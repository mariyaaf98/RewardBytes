using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Users.Features.ToggleUserStatus;

public sealed record ToggleUserStatusCommand(Guid UserId)
    : ICommand<Result<bool>>;
