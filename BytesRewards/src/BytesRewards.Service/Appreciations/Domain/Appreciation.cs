using AppWeaver.DomainAbstraction.Aggregates;

using BytesRewards.Service.Common;

namespace BytesRewards.Service.Appreciations.Domain;

public class Appreciation : BaseEntity, IAggregateRoot
{
    public Guid FromUserId { get; set; }

    public Guid ToUserId { get; set; }

    public string Message { get; set; } = string.Empty;

    /// <summary>Sender's full name — snapshotted at creation.</summary>
    public string FromUserName { get; set; } = string.Empty;

    /// <summary>Recipient's full name — snapshotted at creation.</summary>
    public string ToUserName { get; set; } = string.Empty;
}