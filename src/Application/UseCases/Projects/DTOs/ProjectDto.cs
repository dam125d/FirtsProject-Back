namespace Intap.FirstProject.Application.UseCases.Projects.DTOs;

public sealed record ProjectDto(
    Guid         Id,
    string       Name,
    string       Client,
    string       Status,
    string?      Description,
    List<string> Scope,
    int          TeamMemberCount,
    int          ResourceUtilization,
    string       StartDate,
    string?      EndDate
);

public sealed record ProjectSummaryDto(
    Guid                  Id,
    string                Name,
    string                Client,
    string                Status,
    string                StartDate,
    string?               EndDate,
    int                   TeamMemberCount,
    int                   Progress,
    List<MemberAvatarDto> MemberAvatars
);

public sealed record MemberAvatarDto(string Initials, string Color);

public sealed record TeamMemberDto(
    Guid   Id,
    string Name,
    string Role,
    string AccessLevel,
    string AvatarInitials,
    string Email
);
