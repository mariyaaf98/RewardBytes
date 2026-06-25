using BytesRewards.Service.Designations.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BytesRewards.Service.Designations.Persistence;

public sealed class DesignationConfiguration : IEntityTypeConfiguration<Designation>
{
    public void Configure(EntityTypeBuilder<Designation> builder)
    {
        builder.ToTable("Designations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}
