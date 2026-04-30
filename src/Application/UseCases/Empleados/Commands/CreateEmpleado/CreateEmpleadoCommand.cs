using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Empleados.Commands.CreateEmpleado;

public sealed record CreateEmpleadoCommand(
    string  Identificacion,
    string  Nombres,
    string  Apellidos,
    string  Correo,
    string? Telefono,
    string  Cargo
) : ICommand<int>;
