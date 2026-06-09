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
    }
}