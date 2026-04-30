using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Domain.Projects;
using Intap.FirstProject.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

internal sealed class ProjectReadRepository(AppDbContext context) : IProjectReadRepository
{
    public Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Projects
            .Include(p => p.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

    public Task<List<Project>> GetAllWithMembersAsync(CancellationToken ct = default) =>
        context.Projects
            .Include(p => p.Members)
                .ThenInclude(m => m.User)
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

    public async Task<List<(ProjectMember Member, User User)>> GetMembersWithUsersAsync(
        Guid projectId,
        CancellationToken ct = default)
    {
        List<ProjectMember> members = await context.ProjectMembers
            .Include(pm => pm.User)
            .Where(pm => pm.ProjectId == projectId)
            .ToListAsync(ct);

        return members.Select(pm => (pm, pm.User)).ToList();
    }
}
