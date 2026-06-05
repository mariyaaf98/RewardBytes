using AppWeaver.DomainAbstraction.Aggregates;

using BytesRewards.Service.Common;

namespace BytesRewards.Service.RewardCategories.Domain;

public class RewardCategory
    : BaseEntity,
      IAggregateRoot
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Bytes { get; set; }

    public bool IsActive { get; set; }
}