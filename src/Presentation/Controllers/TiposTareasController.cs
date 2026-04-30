using Intap.FirstProject.API.Common;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposTareas.Commands.ChangeTipoTareaEstado;
using Intap.FirstProject.Application.UseCases.TiposTareas.Commands.CreateTipoTarea;
using Intap.FirstProject.Application.UseCases.TiposTareas.Commands.UpdateTipoTarea;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;
using Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTipoTareaById;
using Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTiposTareas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/tipos-tareas")]
public sealed class TiposTareasController(
    IQueryHandler<GetTiposTareasQuery, PagedResult<TipoTareaDto>>         getAllHandler,
    IQueryHandler<GetTipoTareaByIdQuery, TipoTareaDto>                   getByIdHandler,
    ICommandHandler<CreateTipoTareaCommand, Guid>                        createHandler,
    ICommandHandler<UpdateTipoTareaCommand, Guid>                        updateHandler,
    ICommandHandler<ChangeTipoTareaEstadoCommand, TipoTareaEstadoDto>    changeEstadoHandler
) : ApiController
{
    [HttpGet]
    [Authorize(Policy = "view_tipos_tareas")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int     page     = 1,
        [FromQuery] int     pageSize = 20,
        [FromQuery] bool?   estado   = null,
        [FromQuery] string? nombre   = null,
        CancellationToken   ct       = default)
    {
        Result<PagedResult<TipoTareaDto>> result = await getAllHandler.Handle(
            new GetTiposTareasQuery(page, pageSize, estado, nombre),
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
    [Authorize(Policy = "view_tipos_tareas")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await getByIdHandler.Handle(new GetTipoTareaByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "create_tipos_tareas")]
    public async Task<IActionResult> Create(
        [FromBody] CreateTipoTareaRequest request,
        CancellationToken                ct)
    {
        Result<Guid> result = await createHandler.Handle(
            new CreateTipoTareaCommand(request.Nombre, request.Descripcion),
            ct);

        if (result.IsFailure) return HandleResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "edit_tipos_tareas")]
    public async Task<IActionResult> Update(
        Guid                           id,
        [FromBody] UpdateTipoTareaRequest request,
        CancellationToken              ct)
    {
        Result<Guid> result = await updateHandler.Handle(
            new UpdateTipoTareaCommand(id, request.Nombre, request.Descripcion),
            ct);

        if (result.IsFailure) return HandleResult(result);

        return HandleResult(await getByIdHandler.Handle(new GetTipoTareaByIdQuery(id), ct));
    }

    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = "delete_tipos_tareas")]
    public async Task<IActionResult> ChangeEstado(
        Guid                                 id,
        [FromBody] ChangeTipoTareaEstadoRequest request,
        CancellationToken                    ct) =>
        HandleResult(await changeEstadoHandler.Handle(
            new ChangeTipoTareaEstadoCommand(id, request.Estado),
            ct));
}

// ─── Request models ─────────────────────────────────────────────────────────

public sealed record CreateTipoTareaRequest(string Nombre, string? Descripcion);

public sealed record UpdateTipoTareaRequest(string Nombre, string? Descripcion);

public sealed record ChangeTipoTareaEstadoRequest(bool Estado);
