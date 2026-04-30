using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Categorias.DTOs;
using Intap.FirstProject.Application.UseCases.Categorias.Errors;
using Intap.FirstProject.Domain.Categorias;

namespace Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategoriaById;

internal sealed class GetCategoriaByIdQueryHandler(
    ICategoriaReadRepository repository,
    IMapper                  mapper
) : IQueryHandler<GetCategoriaByIdQuery, CategoriaDto>
{
    public async Task<Result<CategoriaDto>> Handle(GetCategoriaByIdQuery query, CancellationToken ct)
    {
        Categoria? categoria = await repository.GetByIdAsync(query.Id, ct);
        if (categoria is null)
            return Result.Failure<CategoriaDto>(CategoriaErrors.NotFound);

        return Result.Success(mapper.Map<CategoriaDto>(categoria));
    }
}
