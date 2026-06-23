using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Users.Features.UpdateCurrentUser;

public sealed record UpdateCurrentUserCommand(
    string KeycloakUserId,
    string FirstName,
    string LastName,
    string PhoneNumber)
    : ICommand<Result<bool>>;
