namespace BytesRewards.Service.Rewards.Features.CreateReward;

public sealed class CreateRewardRequest
{
    public Guid FromUserId { get; set; }

    public Guid ToUserId { get; set; }

    public Guid RewardCategoryId { get; set; }

    public string Reason { get; set; } = string.Empty;
}