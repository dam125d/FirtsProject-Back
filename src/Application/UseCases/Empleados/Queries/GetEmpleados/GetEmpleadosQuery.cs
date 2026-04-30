using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Empleados.DTOs;

namespace Intap.FirstProject.Application.UseCases.Empleados.Queries.GetEmpleados;

public sealed record GetEmpleadosQuery(bool SoloActivos = true) : IQuery<List<EmpleadoDto>>;
