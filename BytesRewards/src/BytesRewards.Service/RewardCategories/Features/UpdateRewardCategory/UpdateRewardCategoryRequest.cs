namespace BytesRewards.Service.RewardCategories.Features.UpdateRewardCategory;

public sealed class UpdateRewardCategoryRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Bytes { get; set; }
}