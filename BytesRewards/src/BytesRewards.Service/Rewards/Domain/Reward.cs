using AppWeaver.DomainAbstraction.Aggregates;

using BytesRewards.Service.Common;

namespace BytesRewards.Service.Rewards.Domain;

public class Reward
    : BaseEntity,
      IAggregateRoot
{
    public Guid FromUserId { get; set; }

    public Guid ToUserId { get; set; }

    public Guid RewardCategoryId { get; set; }

    public string Reason { get; set; } = string.Empty;

    /// <summary>Bytes awarded — snapshotted at creation.</summary>
    public int Bytes { get; set; }

    /// <summary>Category name — snapshotted at creation.</summary>
    public string RewardCategoryName { get; set; } = string.Empty;

    /// <summary>Giver's full name — snapshotted at creation.</summary>
    public string FromUserName { get; set; } = string.Empty;

    /// <summary>Recipient's full name — snapshotted at creation.</summary>
    public string ToUserName { get; set; } = string.Empty;
}