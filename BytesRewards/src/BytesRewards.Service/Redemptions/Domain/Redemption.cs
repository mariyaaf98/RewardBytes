using AppWeaver.DomainAbstraction.Aggregates;

using BytesRewards.Service.Common;

namespace BytesRewards.Service.Redemptions.Domain;

public sealed class Redemption
    : BaseEntity,
      IAggregateRoot
{
    public Guid UserId { get; set; }

    public Guid RewardItemId { get; set; }

    public int RedeemedBytes { get; set; }

    public string Status { get; set; } = "Pending";

    /// <summary>Product name — snapshotted at redemption time.</summary>
    public string ProductName { get; set; } = string.Empty;
}