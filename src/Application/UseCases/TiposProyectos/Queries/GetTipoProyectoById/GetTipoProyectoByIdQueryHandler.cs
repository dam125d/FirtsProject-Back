using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Errors;
using Intap.FirstProject.Domain.TiposProyectos;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTipoProyectoById;

internal sealed class GetTipoProyectoByIdQueryHandler(
    ITipoProyectoReadRepository repository,
    IMapper                     mapper
) : IQueryHandler<GetTipoProyectoByIdQuery, TipoProyectoDto>
{
    public async Task<Result<TipoProyectoDto>> Handle(
        GetTipoProyectoByIdQuery query,
        CancellationToken        ct)
    {
        TipoProyecto? tipoProyecto = await repository.GetByIdAsync(query.Id, ct);
        if (tipoProyecto is null)
            return Result.Failure<TipoProyectoDto>(TipoProyectoErrors.NotFound);

        return Result.Success(mapper.Map<TipoProyectoDto>(tipoProyecto));
    }
}
