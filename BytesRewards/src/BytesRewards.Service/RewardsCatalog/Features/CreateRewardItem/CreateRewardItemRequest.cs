namespace BytesRewards.Service.RewardsCatalog.Features.CreateRewardItem;

public sealed class CreateRewardItemRequest
{
    public string ProductCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int RequiredBytes { get; set; }

    public bool IsActive { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}