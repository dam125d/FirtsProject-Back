using Intap.FirstProject.API.Common;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.SubCategorias.Commands.CreateSubCategoria;
using Intap.FirstProject.Application.UseCases.SubCategorias.Commands.DeactivateSubCategoria;
using Intap.FirstProject.Application.UseCases.SubCategorias.Commands.UpdateSubCategoria;
using Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;
using Intap.FirstProject.Application.UseCases.SubCategorias.Queries.GetSubCategoriaById;
using Intap.FirstProject.Application.UseCases.SubCategorias.Queries.GetSubCategorias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/[controller]")]
public class SubCategoriasController(
    IQueryHandler<GetSubCategoriasQuery, List<SubCategoriaDto>>   getAllHandler,
    IQueryHandler<GetSubCategoriaByIdQuery, SubCategoriaDto>      getByIdHandler,
    ICommandHandler<CreateSubCategoriaCommand, Guid>              createHandler,
    ICommandHandler<UpdateSubCategoriaCommand>                    updateHandler,
    ICommandHandler<DeactivateSubCategoriaCommand>                deactivateHandler
) : ApiController
{
    [HttpGet]
    [Authorize(Policy = "view_subcategorias")]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool  soloActivas = true,
        [FromQuery] Guid? categoriaId = null,
        CancellationToken ct = default) =>
        HandleResult(await getAllHandler.Handle(new GetSubCategoriasQuery(soloActivas, categoriaId), ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "view_subcategorias")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await getByIdHandler.Handle(new GetSubCategoriaByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "create_subcategorias")]
    public async Task<IActionResult> Create([FromBody] CreateSubCategoriaRequest request, CancellationToken ct)
    {
        CreateSubCategoriaCommand command = new(request.Nombre, request.Descripcion, request.CategoriaId);
        Result<Guid> result = await createHandler.Handle(command, ct);
        if (result.IsFailure) return HandleResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "edit_subcategorias")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubCategoriaRequest request, CancellationToken ct)
    {
        UpdateSubCategoriaCommand command = new(id, request.Nombre, request.Descripcion, request.CategoriaId, request.Estado);
        return HandleResult(await updateHandler.Handle(command, ct));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "delete_subcategorias")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct) =>
        HandleResult(await deactivateHandler.Handle(new DeactivateSubCategoriaCommand(id), ct));
}
