namespace BytesRewards.Service.RewardCategories.Features.GetRewardCategories;

public sealed class RewardCategoryResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Bytes { get; set; }

    public bool IsActive { get; set; }
}