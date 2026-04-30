using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Queries.GetSubCategoriaById;

public sealed record GetSubCategoriaByIdQuery(Guid Id) : IQuery<SubCategoriaDto>;
