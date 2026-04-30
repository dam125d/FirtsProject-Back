using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

internal sealed class UserWriteRepository(AppDbContext context) : IUserWriteRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

    public void Add(User user) => context.Users.Add(user);
}
