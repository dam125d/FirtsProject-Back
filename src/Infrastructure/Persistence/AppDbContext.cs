using Microsoft.EntityFrameworkCore;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.Empleados;
using Intap.FirstProject.Domain.SubCategorias;
using Intap.FirstProject.Domain.Permissions;
using Intap.FirstProject.Domain.Roles;
using Intap.FirstProject.Domain.Users;
using Intap.FirstProject.Domain.Projects;
using Intap.FirstProject.Domain.Tasks;
using Intap.FirstProject.Domain.TiposProyectos;
using Intap.FirstProject.Domain.TiposTareas;

namespace Intap.FirstProject.Infrastructure.Persistence
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Categoria>      Categorias      => Set<Categoria>();
        public DbSet<Empleado>       Empleados       => Set<Empleado>();
        public DbSet<TipoProyecto>   TiposProyecto   => Set<TipoProyecto>();
        public DbSet<TipoTarea>      TiposTarea      => Set<TipoTarea>();
        public DbSet<SubCategoria>   SubCategorias   => Set<SubCategoria>();
        public DbSet<User>           Users           => Set<User>();
        public DbSet<Project>        Projects        => Set<Project>();
        public DbSet<ProjectMember>  ProjectMembers  => Set<ProjectMember>();
        public DbSet<TaskEntry>      Tasks           => Set<TaskEntry>();
        public DbSet<Role>           Roles           => Set<Role>();
        public DbSet<Permission>     Permissions     => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
