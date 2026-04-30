using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class RoleReadRepository(AppDbContext context) : IRoleReadRepository
{
    public Task<List<Role>> GetAllAsync(CancellationToken ct) =>
        context.Roles.AsNoTracking().ToListAsync(ct);

    public Task<Role?> GetByIdAsync(Guid id, CancellationToken ct) =>
        context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken ct) =>
        context.Roles.Include(r => r.Permissions).AsNoTracking()
                     .FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct) =>
        context.Roles.AnyAsync(r => r.Name == name, ct);

    public Task<bool> HasUsersAssignedAsync(Guid roleId, CancellationToken ct) =>
        context.Users.AnyAsync(u => u.RoleId == roleId, ct);
}
