using Intap.FirstProject.API.Common;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.UseCases.Users.Commands.CreateUser;
using Intap.FirstProject.Application.UseCases.Users.Commands.DeleteUser;
using Intap.FirstProject.Application.UseCases.Users.Commands.ToggleUserStatus;
using Intap.FirstProject.Application.UseCases.Users.Commands.UpdateUser;
using Intap.FirstProject.Application.UseCases.Users.DTOs;
using Intap.FirstProject.Application.UseCases.Users.Queries.GetAllUsers;
using Intap.FirstProject.Application.UseCases.Users.Queries.GetUserById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/[controller]")]
public class UsersController(
    IQueryHandler<GetAllUsersQuery, PagedResult<UserDto>>   getAllHandler,
    IQueryHandler<GetUserByIdQuery, UserDto>                getByIdHandler,
    ICommandHandler<CreateUserCommand, Guid>                createHandler,
    ICommandHandler<UpdateUserCommand>                      updateHandler,
    ICommandHandler<DeleteUserCommand>                      deleteHandler,
    ICommandHandler<ToggleUserStatusCommand>                toggleHandler
) : ApiController
{
    [HttpPost("search")]
    [Authorize(Policy = "view_users")]
    public async Task<IActionResult> Search([FromBody] GetAllUsersQuery query, CancellationToken ct) =>
        HandleResult(await getAllHandler.Handle(query, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "view_users")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await getByIdHandler.Handle(new GetUserByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "create_users")]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        Result<Guid> result = await createHandler.Handle(command, ct);
        if (result.IsFailure) return HandleResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "edit_users")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command, CancellationToken ct) =>
        HandleResult(await updateHandler.Handle(command with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "delete_users")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        HandleResult(await deleteHandler.Handle(new DeleteUserCommand(id), ct));

    [HttpPatch("{id:guid}/toggle-status")]
    [Authorize(Policy = "edit_users")]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken ct) =>
        HandleResult(await toggleHandler.Handle(new ToggleUserStatusCommand(id), ct));
}
