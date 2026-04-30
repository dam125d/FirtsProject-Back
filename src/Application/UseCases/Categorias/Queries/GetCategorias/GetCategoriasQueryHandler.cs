using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Categorias.DTOs;
using Intap.FirstProject.Domain.Categorias;

namespace Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategorias;

internal sealed class GetCategoriasQueryHandler(
    ICategoriaReadRepository repository,
    IMapper                  mapper
) : IQueryHandler<GetCategoriasQuery, List<CategoriaDto>>
{
    public async Task<Result<List<CategoriaDto>>> Handle(GetCategoriasQuery query, CancellationToken ct)
    {
        List<Categoria> categorias = await repository.GetAllAsync(query.SoloActivas, ct);
        return Result.Success(mapper.Map<List<CategoriaDto>>(categorias));
    }
}
