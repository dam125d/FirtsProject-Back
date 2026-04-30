using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTiposTareas;

public sealed record GetTiposTareasQuery(
    int     Page     = 1,
    int     PageSize = 20,
    bool?   Estado   = null,
    string? Nombre   = null
) : IQuery<PagedResult<TipoTareaDto>>;
