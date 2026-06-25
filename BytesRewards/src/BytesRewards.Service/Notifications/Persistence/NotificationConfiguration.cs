using BytesRewards.Service.Notifications.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BytesRewards.Service.Notifications.Persistence;

public sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Type).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.IsRead).HasDefaultValue(false);
        builder.HasIndex(x => x.UserId); // fast per-user queries
    }
}
