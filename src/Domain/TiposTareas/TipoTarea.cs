using Intap.FirstProject.Domain.Common;

namespace Intap.FirstProject.Domain.TiposTareas;

public sealed class TipoTarea : BaseEntity
{
    private TipoTarea() { }

    public string  Nombre      { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool    Estado      { get; private set; } = true;

    public static TipoTarea Create(string nombre, string? descripcion)
    {
        return new TipoTarea
        {
            Nombre      = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            Estado      = true,
        };
    }

    public void Update(string nombre, string? descripcion)
    {
        Nombre      = nombre.Trim();
        Descripcion = descripcion?.Trim();
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
