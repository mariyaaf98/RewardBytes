using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Leaderboard.Features.GetLeaderboard;

public sealed record GetLeaderboardQuery()
    : IQuery<List<GetLeaderboardResponse>>;