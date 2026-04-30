using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Domain.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class PermissionReadRepository(AppDbContext context) : IPermissionReadRepository
{
    public Task<List<Permission>> GetAllAsync(CancellationToken ct) =>
        context.Permissions.AsNoTracking().ToListAsync(ct);

    public Task<List<Permission>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken ct) =>
        context.Permissions.Where(p => codes.Contains(p.Code))
                           .AsNoTracking().ToListAsync(ct);
}
