using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;
using Intap.FirstProject.Application.UseCases.SubCategorias.Errors;
using Intap.FirstProject.Domain.SubCategorias;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Queries.GetSubCategoriaById;

internal sealed class GetSubCategoriaByIdQueryHandler(
    ISubCategoriaReadRepository repository,
    IMapper                     mapper
) : IQueryHandler<GetSubCategoriaByIdQuery, SubCategoriaDto>
{
    public async Task<Result<SubCategoriaDto>> Handle(GetSubCategoriaByIdQuery query, CancellationToken ct)
    {
        SubCategoria? subCategoria = await repository.GetByIdAsync(query.Id, ct);
        if (subCategoria is null)
            return Result.Failure<SubCategoriaDto>(SubCategoriaErrors.NotFound);

        return Result.Success(mapper.Map<SubCategoriaDto>(subCategoria));
    }
}
