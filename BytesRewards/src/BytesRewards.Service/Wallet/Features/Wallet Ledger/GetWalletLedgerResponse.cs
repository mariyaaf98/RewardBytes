namespace BytesRewards.Service.Wallets.Features.GetWalletLedger;

public sealed class GetWalletLedgerResponse
{
    public Guid RewardId { get; set; }

    public string RewardCategoryName { get; set; } = string.Empty;

    public int Bytes { get; set; }

    public string AwardedBy { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public DateTime AwardedAt { get; set; }
}