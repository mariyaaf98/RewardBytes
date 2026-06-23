namespace BytesRewards.Service.Redemptions.Features.GetRedemptionHistory;

public sealed class GetRedemptionHistoryResponse
{
    public Guid RedemptionId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int RedeemedBytes { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime RedeemedAt { get; set; }
}