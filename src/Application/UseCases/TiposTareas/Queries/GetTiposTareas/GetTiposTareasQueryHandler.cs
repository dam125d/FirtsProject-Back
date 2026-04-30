using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;
using Intap.FirstProject.Application.UseCases.TiposTareas.Errors;
using Intap.FirstProject.Domain.TiposTareas;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTiposTareas;

internal sealed class GetTiposTareasQueryHandler(
    ITipoTareaReadRepository repository,
    IMapper                  mapper
) : IQueryHandler<GetTiposTareasQuery, PagedResult<TipoTareaDto>>
{
    public async Task<Result<PagedResult<TipoTareaDto>>> Handle(
        GetTiposTareasQuery query,
        CancellationToken   ct)
    {
        if (query.Page < 1 || query.PageSize < 1 || query.PageSize > 100)
            return Result.Failure<PagedResult<TipoTareaDto>>(TipoTareaErrors.ValidationError);

        (List<TipoTarea> items, int total) = await repository.GetAllPagedAsync(
            query.Page,
            query.PageSize,
            query.Estado,
            query.Nombre,
            ct);

        List<TipoTareaDto> dtos = mapper.Map<List<TipoTareaDto>>(items);

        return Result.Success(new PagedResult<TipoTareaDto>(
            dtos,
            total,
            query.Page,
            query.PageSize));
    }
}
