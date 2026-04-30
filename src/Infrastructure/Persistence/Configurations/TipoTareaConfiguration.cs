using Intap.FirstProject.Domain.TiposTareas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intap.FirstProject.Infrastructure.Persistence.Configurations;

internal sealed class TipoTareaConfiguration : IEntityTypeConfiguration<TipoTarea>
{
    public void Configure(EntityTypeBuilder<TipoTarea> builder)
    {
        builder.ToTable("TiposTarea");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Descripcion)
            .HasMaxLength(500);

        builder.Property(t => t.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        // Partial unique index: name unique among active records only (case-insensitive via ILIKE in queries)
        builder.HasIndex(t => t.Nombre)
            .IsUnique()
            .HasFilter("\"Estado\" = TRUE")
            .HasDatabaseName("UQ_TiposTarea_Nombre_Activos");

        builder.HasIndex(t => t.Estado)
            .HasDatabaseName("IX_TiposTarea_Estado");
    }
}
