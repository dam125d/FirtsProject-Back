using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Projects.DTOs;
using Intap.FirstProject.Domain.Projects;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectMembers;

internal sealed class GetProjectMembersQueryHandler(
    IProjectReadRepository projectReadRepository
) : IQueryHandler<GetProjectMembersQuery, List<TeamMemberDto>>
{
    public async Task<Result<List<TeamMemberDto>>> Handle(GetProjectMembersQuery query, CancellationToken cancellationToken)
    {
        List<(ProjectMember Member, User User)> membersWithUsers =
            await projectReadRepository.GetMembersWithUsersAsync(query.ProjectId, cancellationToken);

        if (membersWithUsers.Count == 0)
        {
            // Verify the project exists — could be no members or not found
            // We return an empty list either way; caller can validate existence separately
        }

        List<TeamMemberDto> dtos = membersWithUsers
            .Select(pair => new TeamMemberDto(
                Id:            pair.Member.Id,
                Name:          pair.User.FullName,
                Role:          pair.User.AssignedRole?.Name ?? string.Empty,
                AccessLevel:   MapAccessLevel(pair.Member.AccessLevel),
                AvatarInitials: GetInitials(pair.User.FullName),
                Email:         pair.User.Email))
            .ToList();

        return Result.Success(dtos);
    }

    private static string MapAccessLevel(AccessLevel level) => level switch
    {
        AccessLevel.ReadOnly    => "readOnly",
        AccessLevel.Contributor => "contributor",
        AccessLevel.Admin       => "admin",
        _                       => "readOnly",
    };

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
