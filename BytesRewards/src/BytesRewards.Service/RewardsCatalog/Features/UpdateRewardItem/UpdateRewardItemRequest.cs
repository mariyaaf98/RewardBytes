namespace BytesRewards.Service.RewardsCatalog.Features.UpdateRewardItem;

public sealed class UpdateRewardItemRequest
{
    public Guid Id { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int RequiredBytes { get; set; }

    public bool IsActive { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}