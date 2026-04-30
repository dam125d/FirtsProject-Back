using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Categorias.DTOs;

namespace Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategoriaById;

public sealed record GetCategoriaByIdQuery(Guid Id) : IQuery<CategoriaDto>;
