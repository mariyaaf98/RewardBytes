using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Users.Features.GetCurrentUser;

public sealed record GetCurrentUserQuery(
    string KeycloakUserId)
    : IQuery<GetCurrentUserResponse>;
