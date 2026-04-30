using Intap.FirstProject.Domain.Roles;
using Intap.FirstProject.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intap.FirstProject.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.RoleId).IsRequired();
        builder.HasOne(u => u.AssignedRole)
               .WithMany()
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.Property(u => u.Identification).IsRequired(false).HasMaxLength(20).HasDefaultValue(string.Empty);
        builder.Property(u => u.Phone).IsRequired(false).HasMaxLength(20).HasDefaultValue(string.Empty);
        builder.Property(u => u.Position).IsRequired(false).HasMaxLength(100).HasDefaultValue(string.Empty);
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
