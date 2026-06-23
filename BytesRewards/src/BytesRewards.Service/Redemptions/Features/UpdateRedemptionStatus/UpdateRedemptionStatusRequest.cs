namespace BytesRewards.Service.Redemptions.Features.UpdateRedemptionStatus;

public sealed class UpdateRedemptionStatusRequest
{
    public Guid RedemptionId { get; set; }

    public string Status { get; set; } = string.Empty;
}