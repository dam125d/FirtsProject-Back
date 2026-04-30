using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Queries.GetSubCategorias;

public sealed record GetSubCategoriasQuery(
    bool  SoloActivas = true,
    Guid? CategoriaId = null
) : IQuery<List<SubCategoriaDto>>;
