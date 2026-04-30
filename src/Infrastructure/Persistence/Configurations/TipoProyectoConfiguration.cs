using Intap.FirstProject.Domain.TiposProyectos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intap.FirstProject.Infrastructure.Persistence.Configurations;

internal sealed class TipoProyectoConfiguration : IEntityTypeConfiguration<TipoProyecto>
{
    public void Configure(EntityTypeBuilder<TipoProyecto> builder)
    {
        builder.ToTable("TiposProyecto");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Descripcion)
            .HasMaxLength(500);

        builder.Property(t => t.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        // Partial unique index: name unique among active records only
        builder.HasIndex(t => t.Nombre)
            .IsUnique()
            .HasFilter("\"Estado\" = TRUE")
            .HasDatabaseName("UQ_TiposProyecto_Nombre_Activos");

        builder.HasIndex(t => t.Estado)
            .HasDatabaseName("IX_TiposProyecto_Estado");
    }
}
