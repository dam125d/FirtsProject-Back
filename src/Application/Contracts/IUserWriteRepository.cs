using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.Contracts;

public interface IUserWriteRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(User user);
}
