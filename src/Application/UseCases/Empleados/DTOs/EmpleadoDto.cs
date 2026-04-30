namespace Intap.FirstProject.Application.UseCases.Empleados.DTOs;

public sealed record EmpleadoDto(
    int       Id,
    string    Identificacion,
    string    Nombres,
    string    Apellidos,
    string    Correo,
    string?   Telefono,
    string    Cargo,
    bool      Estado,
    DateTime  CreatedAt,
    DateTime? UpdatedAt);
