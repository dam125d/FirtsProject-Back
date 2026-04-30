using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface IUserReadRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRoleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId, CancellationToken cancellationToken = default);
    Task<(List<User> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        bool?   isActive,
        int     page,
        int     pageSize,
        string  sortBy,
        string  sortOrder,
        CancellationToken cancellationToken = default);
}
