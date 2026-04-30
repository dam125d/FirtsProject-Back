using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Errors;
using Intap.FirstProject.Domain.TiposProyectos;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTiposProyectos;

internal sealed class GetTiposProyectosQueryHandler(
    ITipoProyectoReadRepository repository,
    IMapper                     mapper
) : IQueryHandler<GetTiposProyectosQuery, PagedResult<TipoProyectoDto>>
{
    public async Task<Result<PagedResult<TipoProyectoDto>>> Handle(
        GetTiposProyectosQuery query,
        CancellationToken      ct)
    {
        if (query.Page < 1 || query.PageSize < 1 || query.PageSize > 100)
            return Result.Failure<PagedResult<TipoProyectoDto>>(TipoProyectoErrors.ValidationError);

        (List<TipoProyecto> items, int total) = await repository.GetAllPagedAsync(
            query.Page,
            query.PageSize,
            query.Estado,
            query.Nombre,
            ct);

        List<TipoProyectoDto> dtos = mapper.Map<List<TipoProyectoDto>>(items);

        return Result.Success(new PagedResult<TipoProyectoDto>(
            dtos,
            total,
            query.Page,
            query.PageSize));
    }
}
