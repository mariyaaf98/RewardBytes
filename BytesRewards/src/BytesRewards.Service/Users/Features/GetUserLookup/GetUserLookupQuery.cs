using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Users.Features.GetUserLookup;

public sealed record GetUserLookupQuery()
    : IQuery<Result<List<UserLookupResponse>>>;