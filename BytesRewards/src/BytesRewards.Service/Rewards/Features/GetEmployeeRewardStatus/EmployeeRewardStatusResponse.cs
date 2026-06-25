namespace BytesRewards.Service.Rewards.Features.GetEmployeeRewardStatus;

public sealed class EmployeeRewardStatusResponse
{
    public List<EmployeeRewardSummary> Rewarded    { get; set; } = [];
    public List<EmployeeRewardSummary> NotRewarded { get; set; } = [];
}

public sealed class EmployeeRewardSummary
{
    public Guid    Id                      { get; set; }
    public string  FullName                { get; set; } = string.Empty;
    public string  DepartmentName          { get; set; } = string.Empty;
    public string  DesignationName         { get; set; } = string.Empty;

    /// <summary>Most recent reward date — populated for the Rewarded list only.</summary>
    public DateTime? LastRewardedAt        { get; set; }

    /// <summary>Category name of the most recent reward.</summary>
    public string    LastRewardCategoryName { get; set; } = string.Empty;

    /// <summary>Bytes of the most recent reward.</summary>
    public int       LastRewardBytes        { get; set; }

    /// <summary>Number of rewards received in the last 6 months.</summary>
    public int TotalRewards { get; set; }
}
