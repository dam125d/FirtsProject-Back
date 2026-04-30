using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Empleados.DTOs;
using Intap.FirstProject.Domain.Empleados;

namespace Intap.FirstProject.Application.UseCases.Empleados.Queries.GetEmpleados;

internal sealed class GetEmpleadosQueryHandler(
    IEmpleadoReadRepository repository,
    IMapper                   mapper
) : IQueryHandler<GetEmpleadosQuery, List<EmpleadoDto>>
{
    public async Task<Result<List<EmpleadoDto>>> Handle(GetEmpleadosQuery query, CancellationToken ct)
    {
        List<Empleado> empleados = await repository.GetAllAsync(query.SoloActivos, ct);
        return Result.Success(mapper.Map<List<EmpleadoDto>>(empleados));
    }
}
