using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using BytesRewards.Service.RewardsCatalog.Domain;

namespace BytesRewards.Service.RewardsCatalog.Persistence;

public sealed class RewardItemConfiguration
    : IEntityTypeConfiguration<RewardItem>
{
    public void Configure(
        EntityTypeBuilder<RewardItem> builder)
    {
        builder.ToTable("RewardItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.RequiredBytes)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}