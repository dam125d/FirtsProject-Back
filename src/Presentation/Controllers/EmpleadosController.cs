using Intap.FirstProject.API.Common;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Empleados.Commands.CreateEmpleado;
using Intap.FirstProject.Application.UseCases.Empleados.Commands.DeleteEmpleado;
using Intap.FirstProject.Application.UseCases.Empleados.Commands.UpdateEmpleado;
using Intap.FirstProject.Application.UseCases.Empleados.DTOs;
using Intap.FirstProject.Application.UseCases.Empleados.Queries.GetEmpleadoById;
using Intap.FirstProject.Application.UseCases.Empleados.Queries.GetEmpleados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/[controller]")]
public class EmpleadosController(
    IQueryHandler<GetEmpleadosQuery, List<EmpleadoDto>>   getAllHandler,
    IQueryHandler<GetEmpleadoByIdQuery, EmpleadoDto>        getByIdHandler,
    ICommandHandler<CreateEmpleadoCommand, int>             createHandler,
    ICommandHandler<UpdateEmpleadoCommand>                  updateHandler,
    ICommandHandler<DeleteEmpleadoCommand>                  deleteHandler
) : ApiController
{
    [HttpGet]
    [Authorize(Policy = "empleados:read")]
    public async Task<IActionResult> GetAll([FromQuery] bool soloActivos = true, CancellationToken ct = default) =>
        HandleResult(await getAllHandler.Handle(new GetEmpleadosQuery(soloActivos), ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = "empleados:read")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        HandleResult(await getByIdHandler.Handle(new GetEmpleadoByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "empleados:create")]
    public async Task<IActionResult> Create([FromBody] CreateEmpleadoCommand command, CancellationToken ct)
    {
        Result<int> result = await createHandler.Handle(command, ct);
        if (result.IsFailure) return HandleResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "empleados:update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmpleadoCommand command, CancellationToken ct) =>
        HandleResult(await updateHandler.Handle(command with { Id = id }, ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "empleados:delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        HandleResult(await deleteHandler.Handle(new DeleteEmpleadoCommand(id), ct));
}
