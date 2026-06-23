namespace BytesRewards.Service.RewardsCatalog.Features.GetRewardItemById;

public sealed class GetRewardItemByIdResponse
{
    public Guid Id { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int RequiredBytes { get; set; }

    public bool IsActive { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}