using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Users.Features.ChangePassword;

public sealed record ChangePasswordCommand(
    string KeycloakUserId,
    string CurrentPassword,
    string NewPassword)
    : ICommand<Result<bool>>;
