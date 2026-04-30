using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Application.Contracts;

public interface IRoleWriteRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Role role);
    void Remove(Role role);
}
