using Microsoft.EntityFrameworkCore;

using AppWeaver.Mediator.Interfaces;

using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Leaderboard.Features.GetLeaderboard;

public sealed class GetLeaderboardQueryHandler(
    ApplicationDbContext context)
    : IQueryHandler<
        GetLeaderboardQuery,
        List<GetLeaderboardResponse>>
{
    public async ValueTask<List<GetLeaderboardResponse>> Handle(
        GetLeaderboardQuery request,
        CancellationToken ct)
    {
        var leaderboard =
            await context.Rewards
                .GroupBy(x => x.ToUserId)
                .Select(x => new
                {
                    UserId = x.Key,

                    TotalEarnedBytes = x.Sum(r => r.Bytes)
                })
                .OrderByDescending(x =>
                    x.TotalEarnedBytes)
                .ToListAsync(ct);

        return leaderboard
            .Select((x, index) =>
                new GetLeaderboardResponse
                {
                    Rank = index + 1,

                    UserId = x.UserId,

                    EmployeeName =
                        context.Users
                            .Where(u =>
                                u.Id == x.UserId)
                            .Select(u =>
                                u.FirstName + " " + u.LastName)
                            .FirstOrDefault() ?? string.Empty,

                    TotalEarnedBytes =
                        x.TotalEarnedBytes
                })
            .ToList();
    }
}