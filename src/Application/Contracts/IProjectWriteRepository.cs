using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.Contracts;

public interface IProjectWriteRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task           AddAsync(Project project, CancellationToken ct = default);
    void           Update(Project project);
}
