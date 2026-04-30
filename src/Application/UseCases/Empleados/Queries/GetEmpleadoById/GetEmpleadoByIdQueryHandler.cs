using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Empleados.DTOs;
using Intap.FirstProject.Application.UseCases.Empleados.Errors;
using Intap.FirstProject.Domain.Empleados;

namespace Intap.FirstProject.Application.UseCases.Empleados.Queries.GetEmpleadoById;

internal sealed class GetEmpleadoByIdQueryHandler(
    IEmpleadoReadRepository repository,
    IMapper                   mapper
) : IQueryHandler<GetEmpleadoByIdQuery, EmpleadoDto>
{
    public async Task<Result<EmpleadoDto>> Handle(GetEmpleadoByIdQuery query, CancellationToken ct)
    {
        Empleado? empleado = await repository.GetByIdAsync(query.Id, ct);
        if (empleado is null)
            return Result.Failure<EmpleadoDto>(EmpleadoErrors.NotFound);

        return Result.Success(mapper.Map<EmpleadoDto>(empleado));
    }
}
