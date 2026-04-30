using Intap.FirstProject.Domain.Permissions;
using Intap.FirstProject.Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intap.FirstProject.Infrastructure.Persistence.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    private static readonly Guid AdminRoleId = new("00000000-0000-0000-0000-000000000001");

    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionCode });

        builder.HasIndex(rp => rp.PermissionCode);

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(rp => rp.PermissionCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new { RoleId = AdminRoleId, PermissionCode = "view_users"      },
            new { RoleId = AdminRoleId, PermissionCode = "create_users"    },
            new { RoleId = AdminRoleId, PermissionCode = "edit_users"      },
            new { RoleId = AdminRoleId, PermissionCode = "delete_users"    },
            new { RoleId = AdminRoleId, PermissionCode = "view_projects"   },
            new { RoleId = AdminRoleId, PermissionCode = "create_projects" },
            new { RoleId = AdminRoleId, PermissionCode = "edit_projects"   },
            new { RoleId = AdminRoleId, PermissionCode = "delete_projects" },
            new { RoleId = AdminRoleId, PermissionCode = "view_tasks"      },
            new { RoleId = AdminRoleId, PermissionCode = "create_tasks"    },
            new { RoleId = AdminRoleId, PermissionCode = "edit_tasks"      },
            new { RoleId = AdminRoleId, PermissionCode = "delete_tasks"    },
            new { RoleId = AdminRoleId, PermissionCode = "view_roles"      },
            new { RoleId = AdminRoleId, PermissionCode = "create_roles"    },
            new { RoleId = AdminRoleId, PermissionCode = "edit_roles"      },
            new { RoleId = AdminRoleId, PermissionCode = "delete_roles"    }
        );
    }
}
