using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTiposProyectos;

public sealed record GetTiposProyectosQuery(
    int     Page     = 1,
    int     PageSize = 20,
    bool?   Estado   = null,
    string? Nombre   = null
) : IQuery<PagedResult<TipoProyectoDto>>;
