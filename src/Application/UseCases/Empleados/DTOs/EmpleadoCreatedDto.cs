namespace Intap.FirstProject.Application.UseCases.Empleados.DTOs;

public sealed record EmpleadoCreatedDto(
    int       Id,
    string    Identificacion,
    string    Nombres,
    string    Apellidos,
    string    Correo,
    string?   Telefono,
    string    Cargo,
    DateTime  CreatedAt,
    DateTime? UpdatedAt);
