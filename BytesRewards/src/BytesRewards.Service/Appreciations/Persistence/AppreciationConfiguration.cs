using BytesRewards.Service.Appreciations.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BytesRewards.Service.Appreciations.Infrastructure;

public sealed class AppreciationConfiguration
    : IEntityTypeConfiguration<Appreciation>
{
    public void Configure(
    EntityTypeBuilder<Appreciation> builder)
    {
        builder.ToTable("Appreciations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Message)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.FromUserId).IsRequired();

        builder.Property(x => x.ToUserId).IsRequired();

        builder.Property(x => x.FromUserName)
            .HasMaxLength(200).IsRequired().HasDefaultValue(string.Empty);

        builder.Property(x => x.ToUserName)
            .HasMaxLength(200).IsRequired().HasDefaultValue(string.Empty);
    }
}