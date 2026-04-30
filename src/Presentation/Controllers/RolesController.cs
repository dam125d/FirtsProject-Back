using Intap.FirstProject.API.Common;
using Intap.FirstProject.API.Requests.Roles;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Roles.Commands.AssignRolePermissions;
using Intap.FirstProject.Application.UseCases.Roles.Commands.CreateRole;
using Intap.FirstProject.Application.UseCases.Roles.Commands.DeleteRole;
using Intap.FirstProject.Application.UseCases.Roles.Commands.UpdateRole;
using Intap.FirstProject.Application.UseCases.Roles.DTOs;
using Intap.FirstProject.Application.UseCases.Roles.Queries.GetAllRoles;
using Intap.FirstProject.Application.UseCases.Roles.Queries.GetRoleById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/[controller]")]
public class RolesController(
    IQueryHandler<GetAllRolesQuery, List<RoleDto>>       getAllHandler,
    IQueryHandler<GetRoleByIdQuery, RoleDetailDto>       getByIdHandler,
    ICommandHandler<CreateRoleCommand, Guid>             createHandler,
    ICommandHandler<UpdateRoleCommand>                   updateHandler,
    ICommandHandler<DeleteRoleCommand>                   deleteHandler,
    ICommandHandler<AssignRolePermissionsCommand>        assignPermissionsHandler
) : ApiController
{
    [HttpGet]
    [Authorize(Policy = "view_roles")]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        HandleResult(await getAllHandler.Handle(new GetAllRolesQuery(), ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "view_roles")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await getByIdHandler.Handle(new GetRoleByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "create_roles")]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command, CancellationToken ct)
    {
        Result<Guid> result = await createHandler.Handle(command, ct);
        if (result.IsFailure) return HandleResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "edit_roles")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken ct) =>
        HandleResult(await updateHandler.Handle(new UpdateRoleCommand(id, request.Name, request.Description), ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "delete_roles")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        HandleResult(await deleteHandler.Handle(new DeleteRoleCommand(id), ct));

    [HttpPut("{id:guid}/permissions")]
    [Authorize(Policy = "edit_roles")]
    public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] AssignRolePermissionsRequest request, CancellationToken ct) =>
        HandleResult(await assignPermissionsHandler.Handle(new AssignRolePermissionsCommand(id, request.PermissionCodes), ct));
}
