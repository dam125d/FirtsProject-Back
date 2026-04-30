using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Categorias.DTOs;

namespace Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategorias;

public sealed record GetCategoriasQuery(bool SoloActivas = true) : IQuery<List<CategoriaDto>>;
