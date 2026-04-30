using Intap.FirstProject.API.Common;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Permissions.DTOs;
using Intap.FirstProject.Application.UseCases.Permissions.Queries.GetAllPermissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/[controller]")]
public class PermissionsController(
    IQueryHandler<GetAllPermissionsQuery, List<PermissionDto>> getAllHandler
) : ApiController
{
    [HttpGet]
    [Authorize(Policy = "view_users")]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        HandleResult(await getAllHandler.Handle(new GetAllPermissionsQuery(), ct));
}
