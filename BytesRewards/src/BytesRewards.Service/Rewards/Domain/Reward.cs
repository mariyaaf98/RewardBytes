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
}