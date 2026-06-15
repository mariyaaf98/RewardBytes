using AppWeaver.DomainAbstraction.Aggregates;

using BytesRewards.Service.Common;

namespace BytesRewards.Service.Wallets.Domain;

public class Wallet
    : BaseEntity,
      IAggregateRoot
{
    public Guid UserId { get; set; }

    public int AvailableBytes { get; set; }
}