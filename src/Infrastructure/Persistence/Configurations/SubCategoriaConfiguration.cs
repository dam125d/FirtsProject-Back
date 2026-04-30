using Intap.FirstProject.Domain.SubCategorias;
using Intap.FirstProject.Domain.Categorias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intap.FirstProject.Infrastructure.Persistence.Configurations;

internal sealed class SubCategoriaConfiguration : IEntityTypeConfiguration<SubCategoria>
{
    public void Configure(EntityTypeBuilder<SubCategoria> builder)
    {
        builder.ToTable("SubCategorias");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.Descripcion)
            .HasMaxLength(500);

        builder.Property(s => s.CategoriaId)
            .IsRequired();

        builder.Property(s => s.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        // FK to Categorias — no cascade delete
        builder.HasOne<Categoria>()
            .WithMany()
            .HasForeignKey(s => s.CategoriaId)
            .HasConstraintName("FK_SubCategorias_Categorias")
            .OnDelete(DeleteBehavior.Restrict);

        // Filtered unique index: Nombre + CategoriaId among active records only
        // Note: EF Core does not support filtered indexes via fluent API with HasFilter
        // The filter WHERE Estado = true is applied in the migration directly.
        builder.HasIndex(s => new { s.Nombre, s.CategoriaId })
            .IsUnique()
            .HasFilter("\"Estado\" = true")
            .HasDatabaseName("UQ_SubCategorias_Nombre_CategoriaId_Activas");

        builder.HasIndex(s => s.CategoriaId)
            .HasDatabaseName("IX_SubCategorias_CategoriaId");

        builder.HasIndex(s => s.Estado)
            .HasDatabaseName("IX_SubCategorias_Estado");
    }
}
