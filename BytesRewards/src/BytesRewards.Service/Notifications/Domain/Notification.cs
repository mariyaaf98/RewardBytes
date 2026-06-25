using AppWeaver.DomainAbstraction.Aggregates;
using BytesRewards.Service.Common;

namespace BytesRewards.Service.Notifications.Domain;

public sealed class Notification : BaseEntity, IAggregateRoot
{
    /// <summary>The user who should receive this notification.</summary>
    public Guid   UserId  { get; set; }

    /// <summary>
    /// Notification type:
    /// RewardReceived | AppreciationReceived | AppreciationSent |
    /// RedemptionPending | RedemptionApproved | RedemptionRejected | RedemptionDelivered
    /// </summary>
    public string Type    { get; set; } = string.Empty;

    public string Title   { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool   IsRead  { get; set; }
}
