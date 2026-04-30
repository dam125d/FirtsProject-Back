using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Projects.DTOs;
using Intap.FirstProject.Application.UseCases.Projects.Errors;
using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectById;

internal sealed class GetProjectByIdQueryHandler(
    IProjectReadRepository projectReadRepository
) : IQueryHandler<GetProjectByIdQuery, ProjectDto>
{
    public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery query, CancellationToken cancellationToken)
    {
        Project? project = await projectReadRepository.GetByIdAsync(query.Id, cancellationToken);
        if (project is null)
            return Result.Failure<ProjectDto>(ProjectErrors.NotFound);

        return Result.Success(ToDto(project));
    }

    private static ProjectDto ToDto(Project p) => new(
        p.Id,
        p.Name,
        p.Client,
        MapStatus(p.Status),
        p.Description,
        p.Scope,
        p.Members.Count,
        Math.Min(100, p.Members.Count * 8),
        p.StartDate.ToString("yyyy-MM-dd"),
        p.EndDate?.ToString("yyyy-MM-dd"));

    private static string MapStatus(ProjectStatus s) => s switch
    {
        ProjectStatus.Active   => "active",
        ProjectStatus.Paused   => "paused",
        ProjectStatus.Closed   => "closed",
        ProjectStatus.Archived => "archived",
        _                      => "active",
    };
}
