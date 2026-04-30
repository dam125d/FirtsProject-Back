using AutoMapper;
using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.UseCases.Projects.DTOs;

public sealed class ProjectMappingProfile : Profile
{
    private static readonly string[] AvatarColors =
    [
        "#1E3A5F", "#2E6DA4", "#16A34A", "#D97706", "#DC2626", "#7C3AED",
    ];

    public ProjectMappingProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ConvertUsing((s, _, _) => new ProjectDto(
                s.Id,
                s.Name,
                s.Client,
                MapProjectStatus(s.Status),
                s.Description,
                s.Scope,
                s.Members.Count,
                Math.Min(100, s.Members.Count * 8),
                s.StartDate.ToString("yyyy-MM-dd"),
                s.EndDate.HasValue ? s.EndDate.Value.ToString("yyyy-MM-dd") : null
            ));

        CreateMap<Project, ProjectSummaryDto>()
            .ConvertUsing((s, _, _) => new ProjectSummaryDto(
                s.Id,
                s.Name,
                s.Client,
                MapProjectStatus(s.Status),
                s.StartDate.ToString("yyyy-MM-dd"),
                s.EndDate.HasValue ? s.EndDate.Value.ToString("yyyy-MM-dd") : null,
                s.Members.Count,
                Math.Min(100, s.Members.Count * 8),
                MapAvatars(s)
            ));
    }

    private static string MapProjectStatus(ProjectStatus status) => status switch
    {
        ProjectStatus.Active   => "active",
        ProjectStatus.Paused   => "paused",
        ProjectStatus.Closed   => "closed",
        ProjectStatus.Archived => "archived",
        _                      => "active",
    };

    private static List<MemberAvatarDto> MapAvatars(Project project)
    {
        List<MemberAvatarDto> avatars = [];
        int index = 0;

        foreach (ProjectMember member in project.Members)
        {
            string initials = GetInitials(member.User.FullName);
            string color    = AvatarColors[index % AvatarColors.Length];
            avatars.Add(new MemberAvatarDto(initials, color));
            index++;
        }

        return avatars;
    }

    private static string GetInitials(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return "?";

        string[] parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][0].ToString().ToUpperInvariant(),
            _ => $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant(),
        };
    }
}
