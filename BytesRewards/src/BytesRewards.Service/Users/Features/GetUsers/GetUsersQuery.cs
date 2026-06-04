using AppWeaver.Mediator.Interfaces;

using AppWeaver.Results;

namespace BytesRewards.Service.Users.Features.GetUsers;

public sealed record GetUsersQuery()
    : IQuery<Result<List<UserResponse>>>;