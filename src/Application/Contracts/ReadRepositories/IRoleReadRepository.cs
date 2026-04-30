using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface IRoleReadRepository
{
    Task<List<Role>> GetAllAsync(CancellationToken ct);
    Task<Role?>      GetByIdAsync(Guid id, CancellationToken ct);
    Task<Role?>      GetByIdWithPermissionsAsync(Guid id, CancellationToken ct);
    Task<bool>       ExistsByNameAsync(string name, CancellationToken ct);
    Task<bool>       HasUsersAssignedAsync(Guid roleId, CancellationToken ct);
}
