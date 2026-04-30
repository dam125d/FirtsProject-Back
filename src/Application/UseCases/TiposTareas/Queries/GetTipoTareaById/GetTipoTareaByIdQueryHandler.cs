using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;
using Intap.FirstProject.Application.UseCases.TiposTareas.Errors;
using Intap.FirstProject.Domain.TiposTareas;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTipoTareaById;

internal sealed class GetTipoTareaByIdQueryHandler(
    ITipoTareaReadRepository repository,
    IMapper                  mapper
) : IQueryHandler<GetTipoTareaByIdQuery, TipoTareaDto>
{
    public async Task<Result<TipoTareaDto>> Handle(
        GetTipoTareaByIdQuery query,
        CancellationToken     ct)
    {
        TipoTarea? tipoTarea = await repository.GetByIdAsync(query.Id, ct);
        if (tipoTarea is null)
            return Result.Failure<TipoTareaDto>(TipoTareaErrors.NotFound);

        return Result.Success(mapper.Map<TipoTareaDto>(tipoTarea));
    }
}
