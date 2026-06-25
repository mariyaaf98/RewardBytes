namespace BytesRewards.Service.Redemptions.Features.GetAllRedemptions;

public sealed class GetAllRedemptionsResponse
{
    public Guid   RedemptionId  { get; set; }
    public Guid   UserId        { get; set; }
    public string UserName      { get; set; } = string.Empty;
    public string ProductName   { get; set; } = string.Empty;
    public int    RedeemedBytes { get; set; }
    public string Status        { get; set; } = string.Empty;
    public DateTime RedeemedAt  { get; set; }
}
