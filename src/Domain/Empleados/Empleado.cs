namespace Intap.FirstProject.Domain.Empleados;

public sealed class Empleado
{
    private Empleado() { }

    public int      Id              { get; private set; }
    public string   Identificacion  { get; private set; } = string.Empty;
    public string   Nombres         { get; private set; } = string.Empty;
    public string   Apellidos       { get; private set; } = string.Empty;
    public string   Correo          { get; private set; } = string.Empty;
    public string?  Telefono        { get; private set; }
    public string   Cargo           { get; private set; } = string.Empty;
    public bool     Estado          { get; private set; } = true;
    public DateTime CreatedAt       { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt      { get; private set; }

    public static Empleado Create(
        string identificacion,
        string nombres,
        string apellidos,
        string correo,
        string? telefono,
        string cargo)
    {
        return new Empleado
        {
            Identificacion = identificacion.Trim(),
            Nombres         = nombres.Trim(),
            Apellidos       = apellidos.Trim(),
            Correo          = correo.Trim().ToLower(),
            Telefono        = telefono?.Trim(),
            Cargo           = cargo.Trim(),
            Estado          = true
        };
    }

    public void Update(
        string identificacion,
        string nombres,
        string apellidos,
        string correo,
        string? telefono,
        string cargo)
    {
        Identificacion = identificacion.Trim();
        Nombres         = nombres.Trim();
        Apellidos       = apellidos.Trim();
        Correo          = correo.Trim().ToLower();
        Telefono        = telefono?.Trim();
        Cargo           = cargo.Trim();
        UpdatedAt       = DateTime.UtcNow;
    }

    public void Delete()
    {
        Estado    = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
