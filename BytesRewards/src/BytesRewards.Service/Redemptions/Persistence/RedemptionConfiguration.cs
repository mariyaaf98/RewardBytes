using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using BytesRewards.Service.Redemptions.Domain;

namespace BytesRewards.Service.Redemptions.Persistence;

public sealed class RedemptionConfiguration
    : IEntityTypeConfiguration<Redemption>
{
    public void Configure(
        EntityTypeBuilder<Redemption> builder)
    {
        builder.ToTable("Redemptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.RewardItemId)
            .IsRequired();

        builder.Property(x => x.RedeemedBytes)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ProductName)
            .HasMaxLength(200).IsRequired().HasDefaultValue(string.Empty);
    }
}