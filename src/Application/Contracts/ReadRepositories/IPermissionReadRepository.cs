using Intap.FirstProject.Domain.Permissions;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface IPermissionReadRepository
{
    Task<List<Permission>> GetAllAsync(CancellationToken ct);
    Task<List<Permission>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken ct);
}
