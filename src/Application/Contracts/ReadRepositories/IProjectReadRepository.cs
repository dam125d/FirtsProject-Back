using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface IProjectReadRepository
{
    Task<Project?>       GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Project>>  GetAllWithMembersAsync(CancellationToken ct = default);
    Task<List<(ProjectMember Member, Domain.Users.User User)>> GetMembersWithUsersAsync(Guid projectId, CancellationToken ct = default);
}
