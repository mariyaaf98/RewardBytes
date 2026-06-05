using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using BytesRewards.Service.RewardCategories.Domain;

namespace BytesRewards.Service.RewardCategories.Persistence;

public sealed class RewardCategoryConfiguration
    : IEntityTypeConfiguration<RewardCategory>
{
    public void Configure(
        EntityTypeBuilder<RewardCategory> builder)
    {
        builder.ToTable("RewardCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Bytes)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}