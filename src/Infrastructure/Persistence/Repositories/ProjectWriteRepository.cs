using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

internal sealed class ProjectWriteRepository(AppDbContext context) : IProjectWriteRepository
{
    public Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

    public async Task AddAsync(Project project, CancellationToken ct = default) =>
        await context.Projects.AddAsync(project, ct);

    public void Update(Project project) =>
        context.Projects.Update(project);
}
