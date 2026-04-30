using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class RoleWriteRepository(AppDbContext context) : IRoleWriteRepository
{
    public void Add(Role role) => context.Roles.Add(role);

    public void Remove(Role role) => context.Roles.Remove(role);

    public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
}
