using Intap.FirstProject.API.Common;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Projects.Commands.AddProjectMember;
using Intap.FirstProject.Application.UseCases.Projects.Commands.ArchiveProject;
using Intap.FirstProject.Application.UseCases.Projects.Commands.CreateProject;
using Intap.FirstProject.Application.UseCases.Projects.Commands.DeleteProject;
using Intap.FirstProject.Application.UseCases.Projects.Commands.RemoveProjectMember;
using Intap.FirstProject.Application.UseCases.Projects.Commands.UpdateProject;
using Intap.FirstProject.Application.UseCases.Projects.DTOs;
using Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectById;
using Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectMembers;
using Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectsOverview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

[Route("api/[controller]")]
public class ProjectsController(
    IQueryHandler<GetProjectsOverviewQuery, List<ProjectSummaryDto>>  overviewHandler,
    IQueryHandler<GetProjectByIdQuery, ProjectDto>                    getByIdHandler,
    IQueryHandler<GetProjectMembersQuery, List<TeamMemberDto>>        getMembersHandler,
    ICommandHandler<CreateProjectCommand, Guid>                       createHandler,
    ICommandHandler<UpdateProjectCommand>                             updateHandler,
    ICommandHandler<DeleteProjectCommand>                             deleteHandler,
    ICommandHandler<ArchiveProjectCommand>                            archiveHandler,
    ICommandHandler<AddProjectMemberCommand>                          addMemberHandler,
    ICommandHandler<RemoveProjectMemberCommand>                       removeMemberHandler
) : ApiController
{
    [HttpGet("overview")]
    [Authorize(Policy = "view_projects")]
    public async Task<IActionResult> GetOverview(CancellationToken ct) =>
        HandleResult(await overviewHandler.Handle(new GetProjectsOverviewQuery(), ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "view_projects")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await getByIdHandler.Handle(new GetProjectByIdQuery(id), ct));

    [HttpGet("{id:guid}/members")]
    [Authorize(Policy = "view_projects")]
    public async Task<IActionResult> GetMembers(Guid id, CancellationToken ct) =>
        HandleResult(await getMembersHandler.Handle(new GetProjectMembersQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "create_projects")]
    public async Task<IActionResult> Create([FromBody] CreateProjectCommand command, CancellationToken ct)
    {
        Result<Guid> result = await createHandler.Handle(command, ct);
        if (result.IsFailure) return HandleResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "edit_projects")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectCommand command, CancellationToken ct) =>
        HandleResult(await updateHandler.Handle(command with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "delete_projects")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        HandleResult(await deleteHandler.Handle(new DeleteProjectCommand(id), ct));

    [HttpPost("{id:guid}/archive")]
    [Authorize(Policy = "edit_projects")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct) =>
        HandleResult(await archiveHandler.Handle(new ArchiveProjectCommand(id), ct));

    [HttpPost("{id:guid}/members")]
    [Authorize(Policy = "edit_projects")]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddProjectMemberCommand command, CancellationToken ct)
    {
        Result result = await addMemberHandler.Handle(command with { ProjectId = id }, ct);
        if (result.IsFailure) return HandleResult(result);
        return StatusCode(201);
    }

    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    [Authorize(Policy = "edit_projects")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid memberId, CancellationToken ct) =>
        HandleResult(await removeMemberHandler.Handle(new RemoveProjectMemberCommand(id, memberId), ct));
}
