using AppWeaver.DomainAbstraction.Aggregates;

using BytesRewards.Service.Common;

namespace BytesRewards.Service.RewardsCatalog.Domain;

public sealed class RewardItem
    : BaseEntity,
      IAggregateRoot
{
    public string ProductCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int RequiredBytes { get; set; }

    public bool IsActive { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}