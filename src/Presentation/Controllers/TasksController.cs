using Intap.FirstProject.API.Common;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Tasks.Commands.CreateTask;
using Intap.FirstProject.Application.UseCases.Tasks.Commands.DeleteTask;
using Intap.FirstProject.Application.UseCases.Tasks.Commands.UpdateTask;
using Intap.FirstProject.Application.UseCases.Tasks.DTOs;
using Intap.FirstProject.Application.UseCases.Tasks.Queries.GetAllTasks;
using Intap.FirstProject.Application.UseCases.Tasks.Queries.GetTaskById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/[controller]")]
public class TasksController(
    IQueryHandler<GetAllTasksQuery, PagedResult<TaskDto>>  getAllHandler,
    IQueryHandler<GetTaskByIdQuery, TaskDto>               getByIdHandler,
    ICommandHandler<CreateTaskCommand, Guid>               createHandler,
    ICommandHandler<UpdateTaskCommand>                     updateHandler,
    ICommandHandler<DeleteTaskCommand>                     deleteHandler
) : ApiController
{
    [HttpGet]
    [Authorize(Policy = "view_tasks")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllTasksQuery query, CancellationToken ct) =>
        HandleResult(await getAllHandler.Handle(query, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "view_tasks")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await getByIdHandler.Handle(new GetTaskByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "create_tasks")]
    public async Task<IActionResult> Create([FromBody] CreateTaskCommand command, CancellationToken ct)
    {
        Result<Guid> result = await createHandler.Handle(command, ct);
        if (result.IsFailure) return HandleResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "edit_tasks")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskCommand command, CancellationToken ct) =>
        HandleResult(await updateHandler.Handle(command with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "delete_tasks")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        HandleResult(await deleteHandler.Handle(new DeleteTaskCommand(id), ct));
}
