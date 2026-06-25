using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using BytesRewards.Service.Users.Domain;

namespace BytesRewards.Service.Users.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(500);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId);

        builder.HasOne(x => x.Designation)
            .WithMany()
            .HasForeignKey(x => x.DesignationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}