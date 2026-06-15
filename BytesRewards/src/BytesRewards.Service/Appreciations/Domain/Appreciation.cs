using AppWeaver.DomainAbstraction.Aggregates;

using BytesRewards.Service.Common;

namespace BytesRewards.Service.Appreciations.Domain;

public class Appreciation : BaseEntity, IAggregateRoot
{
    public Guid FromUserId { get; set; }

    public Guid ToUserId { get; set; }

    public string Message { get; set; } = string.Empty;

    // public ICollection<AppreciationLike> Likes { get; set; } = [];
}