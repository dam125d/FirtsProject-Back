using Intap.FirstProject.Domain.Common;

namespace Intap.FirstProject.Domain.SubCategorias;

public sealed class SubCategoria : BaseEntity
{
    private SubCategoria() { }

    public string  Nombre      { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public Guid    CategoriaId { get; private set; }
    public bool    Estado      { get; private set; } = true;

    public static SubCategoria Create(string nombre, string? descripcion, Guid categoriaId)
    {
        return new SubCategoria
        {
            Nombre      = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            CategoriaId = categoriaId,
            Estado      = true,
        };
    }

    public void Update(string nombre, string? descripcion, Guid categoriaId, bool estado)
    {
        Nombre      = nombre.Trim();
        Descripcion = descripcion?.Trim();
        CategoriaId = categoriaId;
        Estado      = estado;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        Estado = false;
        SetUpdatedAt();
    }

    public void Reactivate()
    {
        Estado = true;
        SetUpdatedAt();
    }
}
