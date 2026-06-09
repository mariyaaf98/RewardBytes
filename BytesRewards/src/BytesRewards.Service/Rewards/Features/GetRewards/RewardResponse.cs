namespace BytesRewards.Service.Rewards.Features.GetRewards;

public sealed class RewardResponse
{
    public Guid Id { get; set; }

    public string FromUserName { get; set; } = string.Empty;

    public string ToUserName { get; set; } = string.Empty;

    public string RewardCategoryName { get; set; } = string.Empty;

    public int Bytes { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}