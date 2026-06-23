using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Users.Features.UpdateProfileImage;

public sealed record UpdateProfileImageCommand(
    string KeycloakUserId,
    string ProfileImageUrl)
    : ICommand<Result<bool>>;
