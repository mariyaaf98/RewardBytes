namespace BytesRewards.Service.Redemptions.Features.RedeemReward;

public sealed class RedeemRewardRequest
{
    public Guid UserId { get; set; }

    public Guid RewardItemId { get; set; }
}