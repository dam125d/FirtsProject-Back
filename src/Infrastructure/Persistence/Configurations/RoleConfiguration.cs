using Intap.FirstProject.Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intap.FirstProject.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    private static readonly Guid AdminRoleId = new("00000000-0000-0000-0000-000000000001");
    private static readonly Guid UserRoleId  = new("00000000-0000-0000-0000-000000000002");

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Description).HasMaxLength(300).HasDefaultValue(string.Empty);
        builder.Property(r => r.IsActive).HasDefaultValue(true);
        builder.Property(r => r.IsSystem).HasDefaultValue(false);
        builder.HasIndex(r => r.Name).IsUnique();
        builder.HasMany(r => r.Permissions)
               .WithOne()
               .HasForeignKey(rp => rp.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasData(
            new { Id = AdminRoleId, Name = "Admin", Description = "Full system access",
                  IsActive = true, IsSystem = true,
                  CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = (DateTime?)null },
            new { Id = UserRoleId, Name = "User", Description = "Standard user access",
                  IsActive = true, IsSystem = true,
                  CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = (DateTime?)null }
        );
    }
}
