using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;
using Intap.FirstProject.Domain.SubCategorias;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Queries.GetSubCategorias;

internal sealed class GetSubCategoriasQueryHandler(
    ISubCategoriaReadRepository repository,
    IMapper                     mapper
) : IQueryHandler<GetSubCategoriasQuery, List<SubCategoriaDto>>
{
    public async Task<Result<List<SubCategoriaDto>>> Handle(GetSubCategoriasQuery query, CancellationToken ct)
    {
        List<SubCategoria> subCategorias = await repository.GetAllAsync(query.SoloActivas, query.CategoriaId, ct);
        return Result.Success(mapper.Map<List<SubCategoriaDto>>(subCategorias));
    }
}
