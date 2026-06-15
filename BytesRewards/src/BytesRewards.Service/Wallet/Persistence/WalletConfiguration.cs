using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using BytesRewards.Service.Wallets.Domain;

namespace BytesRewards.Service.Wallets.Persistence;

public sealed class WalletConfiguration
    : IEntityTypeConfiguration<Wallet>
{
    public void Configure(
        EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AvailableBytes)
            .IsRequired();
    }
}