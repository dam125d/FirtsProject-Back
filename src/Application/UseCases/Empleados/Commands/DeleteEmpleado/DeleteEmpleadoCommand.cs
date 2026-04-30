using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Empleados.Commands.DeleteEmpleado;

public sealed record DeleteEmpleadoCommand(int Id) : ICommand;
