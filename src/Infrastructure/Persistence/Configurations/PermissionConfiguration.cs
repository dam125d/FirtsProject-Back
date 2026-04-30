using Intap.FirstProject.Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intap.FirstProject.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Code);

        builder.Property(p => p.Code)
            .HasMaxLength(100);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Module)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasData(
            new { Code = "view_users",      Name = "View Users",      Module = "Users"     },
            new { Code = "create_users",    Name = "Create Users",    Module = "Users"     },
            new { Code = "edit_users",      Name = "Edit Users",      Module = "Users"     },
            new { Code = "delete_users",    Name = "Delete Users",    Module = "Users"     },
            new { Code = "view_projects",   Name = "View Projects",   Module = "Projects"  },
            new { Code = "create_projects", Name = "Create Projects", Module = "Projects"  },
            new { Code = "edit_projects",   Name = "Edit Projects",   Module = "Projects"  },
            new { Code = "delete_projects", Name = "Delete Projects", Module = "Projects"  },
            new { Code = "view_tasks",      Name = "View Tasks",      Module = "Tasks"     },
            new { Code = "create_tasks",    Name = "Create Tasks",    Module = "Tasks"     },
            new { Code = "edit_tasks",      Name = "Edit Tasks",      Module = "Tasks"     },
            new { Code = "delete_tasks",    Name = "Delete Tasks",    Module = "Tasks"     },
            new { Code = "view_roles",      Name = "View Roles",      Module = "Roles"     },
            new { Code = "create_roles",    Name = "Create Roles",    Module = "Roles"     },
            new { Code = "edit_roles",      Name = "Edit Roles",      Module = "Roles"     },
            new { Code = "delete_roles",    Name = "Delete Roles",    Module = "Roles"     }
        );
    }
}
