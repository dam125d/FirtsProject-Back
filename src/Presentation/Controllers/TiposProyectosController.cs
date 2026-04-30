using Intap.FirstProject.API.Common;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.ChangeTipoProyectoEstado;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.CreateTipoProyecto;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.UpdateTipoProyecto;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTipoProyectoById;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTiposProyectos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/tipos-proyectos")]
public sealed class TiposProyectosController(
    IQueryHandler<GetTiposProyectosQuery, PagedResult<TipoProyectoDto>>         getAllHandler,
    IQueryHandler<GetTipoProyectoByIdQuery, TipoProyectoDto>                    getByIdHandler,
    ICommandHandler<CreateTipoProyectoCommand, Guid>                            createHandler,
    ICommandHandler<UpdateTipoProyectoCommand, Guid>                            updateHandler,
    ICommandHandler<ChangeTipoProyectoEstadoCommand, TipoProyectoEstadoDto>     changeEstadoHandler
) : ApiController
{
    [HttpGet]
    [Authorize(Policy = "view_tipos_proyectos")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int     page     = 1,
        [FromQuery] int     pageSize = 20,
        [FromQuery] bool?   estado   = null,
        [FromQuery] string? nombre   = null,
        CancellationToken   ct       = default)
    {
        Result<PagedResult<TipoProyectoDto>> result = await getAllHandler.Handle(
            new GetTiposProyectosQuery(page, pageSize, estado, nombre),
            ct);

        if (result.IsFailure) return HandleResult(result);

        return Ok(new
        {
            data       = result.Value.Items,
            pagination = new
            {
                page      = result.Value.Page,
                pageSize  = result.Value.PageSize,
                total     = result.Value.TotalCount,
            },
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "view_tipos_proyectos")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await getByIdHandler.Handle(new GetTipoProyectoByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "create_tipos_proyectos")]
    public async Task<IActionResult> Create(
        [FromBody] CreateTipoProyectoRequest request,
        CancellationToken                   ct)
    {
        Result<Guid> result = await createHandler.Handle(
            new CreateTipoProyectoCommand(request.Nombre, request.Descripcion),
            ct);

        if (result.IsFailure) return HandleResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "edit_tipos_proyectos")]
    public async Task<IActionResult> Update(
        Guid                              id,
        [FromBody] UpdateTipoProyectoRequest request,
        CancellationToken                 ct)
    {
        Result<Guid> result = await updateHandler.Handle(
            new UpdateTipoProyectoCommand(id, request.Nombre, request.Descripcion),
            ct);

        if (result.IsFailure) return HandleResult(result);

        return HandleResult(await getByIdHandler.Handle(new GetTipoProyectoByIdQuery(id), ct));
    }

    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = "delete_tipos_proyectos")]
    public async Task<IActionResult> ChangeEstado(
        Guid                                    id,
        [FromBody] ChangeTipoProyectoEstadoRequest request,
        CancellationToken                       ct) =>
        HandleResult(await changeEstadoHandler.Handle(
            new ChangeTipoProyectoEstadoCommand(id, request.Estado),
            ct));
}

// ─── Request models ─────────────────────────────────────────────────────────

public sealed record CreateTipoProyectoRequest(string Nombre, string? Descripcion);

public sealed record UpdateTipoProyectoRequest(string Nombre, string? Descripcion);

public sealed record ChangeTipoProyectoEstadoRequest(bool Estado);
