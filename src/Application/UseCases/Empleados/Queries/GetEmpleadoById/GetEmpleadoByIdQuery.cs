using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Empleados.DTOs;

namespace Intap.FirstProject.Application.UseCases.Empleados.Queries.GetEmpleadoById;

public sealed record GetEmpleadoByIdQuery(int Id) : IQuery<EmpleadoDto>;
