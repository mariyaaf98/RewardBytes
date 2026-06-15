namespace BytesRewards.Service.Leaderboard.Features.GetLeaderboard;

public sealed class GetLeaderboardResponse
{
    public int Rank { get; set; }

    public Guid UserId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public int TotalEarnedBytes { get; set; }
}