using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Projects.DTOs;
using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectsOverview;

internal sealed class GetProjectsOverviewQueryHandler(
    IProjectReadRepository projectReadRepository
) : IQueryHandler<GetProjectsOverviewQuery, List<ProjectSummaryDto>>
{
    private static readonly string[] AvatarColors =
    [
        "#1E3A5F", "#2E6DA4", "#16A34A", "#D97706", "#DC2626", "#7C3AED",
    ];

    public async Task<Result<List<ProjectSummaryDto>>> Handle(GetProjectsOverviewQuery query, CancellationToken cancellationToken)
    {
        List<Project> projects = await projectReadRepository.GetAllWithMembersAsync(cancellationToken);
        List<ProjectSummaryDto> dtos = projects.Select(ToSummaryDto).ToList();

        return Result.Success(dtos);
    }

    private static ProjectSummaryDto ToSummaryDto(Project p)
    {
        List<MemberAvatarDto> avatars = p.Members
            .Select((m, i) => new MemberAvatarDto(
                GetInitials(m.User?.FullName ?? string.Empty),
                AvatarColors[i % AvatarColors.Length]))
            .ToList();

        return new ProjectSummaryDto(
            p.Id,
            p.Name,
            p.Client,
            MapStatus(p.Status),
            p.StartDate.ToString("yyyy-MM-dd"),
            p.EndDate?.ToString("yyyy-MM-dd"),
            p.Members.Count,
            Math.Min(100, p.Members.Count * 8),
            avatars);
    }

    private static string MapStatus(ProjectStatus s) => s switch
    {
        ProjectStatus.Active   => "active",
        ProjectStatus.Paused   => "paused",
        ProjectStatus.Closed   => "closed",
        ProjectStatus.Archived => "archived",
        _                      => "active",
    };

    private static string GetInitials(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return "?";
        string[] parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][0].ToString().ToUpperInvariant(),
            _ => $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant(),
        };
    }
}
