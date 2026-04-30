using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Empleados.Commands.UpdateEmpleado;

public sealed record UpdateEmpleadoCommand(
    int     Id,
    string  Identificacion,
    string  Nombres,
    string  Apellidos,
    string  Correo,
    string? Telefono,
    string  Cargo
) : ICommand;
