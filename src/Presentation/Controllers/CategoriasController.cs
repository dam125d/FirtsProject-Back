using Intap.FirstProject.API.Common;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.CreateCategoria;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.DeactivateCategoria;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.UpdateCategoria;
using Intap.FirstProject.Application.UseCases.Categorias.DTOs;
using Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategoriaById;
using Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategorias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/[controller]")]
public class CategoriasController(
    IQueryHandler<GetCategoriasQuery, List<CategoriaDto>>   getAllHandler,
    IQueryHandler<GetCategoriaByIdQuery, CategoriaDto>      getByIdHandler,
    ICommandHandler<CreateCategoriaCommand, Guid>           createHandler,
    ICommandHandler<UpdateCategoriaCommand>                 updateHandler,
    ICommandHandler<DeactivateCategoriaCommand>             deactivateHandler
) : ApiController
{
    [HttpGet]
    [Authorize(Policy = "view_categorias")]
    public async Task<IActionResult> GetAll([FromQuery] bool soloActivas = true, CancellationToken ct = default) =>
        HandleResult(await getAllHandler.Handle(new GetCategoriasQuery(soloActivas), ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "view_categorias")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await getByIdHandler.Handle(new GetCategoriaByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "create_categorias")]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaCommand command, CancellationToken ct)
    {
        Result<Guid> result = await createHandler.Handle(command, ct);
        if (result.IsFailure) return HandleResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "edit_categorias")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoriaCommand command, CancellationToken ct) =>
        HandleResult(await updateHandler.Handle(command with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "delete_categorias")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct) =>
        HandleResult(await deactivateHandler.Handle(new DeactivateCategoriaCommand(id), ct));
}
