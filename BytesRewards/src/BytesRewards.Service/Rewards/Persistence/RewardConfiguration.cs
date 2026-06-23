using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using BytesRewards.Service.Rewards.Domain;

namespace BytesRewards.Service.Rewards.Persistence;

public sealed class RewardConfiguration
    : IEntityTypeConfiguration<Reward>
{
    public void Configure(
        EntityTypeBuilder<Reward> builder)
    {
        builder.ToTable("Rewards");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.Bytes).IsRequired();

        builder.Property(x => x.RewardCategoryName)
            .HasMaxLength(200).IsRequired().HasDefaultValue(string.Empty);

        builder.Property(x => x.FromUserName)
            .HasMaxLength(200).IsRequired().HasDefaultValue(string.Empty);

        builder.Property(x => x.ToUserName)
            .HasMaxLength(200).IsRequired().HasDefaultValue(string.Empty);
    }
}