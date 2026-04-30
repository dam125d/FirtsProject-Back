using Intap.FirstProject.Domain.Empleados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intap.FirstProject.Infrastructure.Persistence.Configurations;

internal sealed class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> builder)
    {
        builder.ToTable("Empleados");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Identificacion)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(e => e.Identificacion)
            .IsUnique()
            .HasDatabaseName("UQ_Empleados_Identificacion");

        builder.Property(e => e.Nombres)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Apellidos)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Correo)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(e => e.Correo)
            .IsUnique()
            .HasDatabaseName("UQ_Empleados_Correo");

        builder.Property(e => e.Telefono)
            .HasMaxLength(20);

        builder.Property(e => e.Cargo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(e => e.Estado)
            .HasDatabaseName("IX_Empleados_Estado");

        builder.HasIndex(e => e.Cargo)
            .HasDatabaseName("IX_Empleados_Cargo");
    }
}
