namespace BytesRewards.Service.RewardCategories.Features.CreateRewardCategory;

public sealed class CreateRewardCategoryRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Bytes { get; set; }
}