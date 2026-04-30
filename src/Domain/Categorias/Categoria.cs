using Intap.FirstProject.Domain.Common;

namespace Intap.FirstProject.Domain.Categorias;

public sealed class Categoria : BaseEntity
{
    private Categoria() { }

    public string  Nombre      { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool    Estado      { get; private set; } = true;

    public static Categoria Create(string nombre, string? descripcion)
    {
        return new Categoria
        {
            Nombre      = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            Estado      = true,
        };
    }

    public void Update(string nombre, string? descripcion, bool estado)
    {
        Nombre      = nombre.Trim();
        Descripcion = descripcion?.Trim();
        Estado      = estado;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        Estado = false;
        SetUpdatedAt();
    }
}
