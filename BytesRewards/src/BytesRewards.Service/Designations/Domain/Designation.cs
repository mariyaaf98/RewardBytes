using AppWeaver.DomainAbstraction.Aggregates;
using BytesRewards.Service.Common;

namespace BytesRewards.Service.Designations.Domain;

public sealed class Designation : BaseEntity, IAggregateRoot
{
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool   IsActive    { get; set; } = true;
}
