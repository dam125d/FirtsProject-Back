using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

internal sealed class UserReadRepository(AppDbContext context) : IUserReadRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(
            u => u.Email == email.ToLowerInvariant() && !u.IsDeleted,
            cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

    public Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default) =>
        context.Users
            .Include(u => u.AssignedRole).ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);

    public Task<User?> GetByIdWithRoleAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Users
            .Include(u => u.AssignedRole).ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, Guid? excludeId, CancellationToken cancellationToken = default) =>
        context.Users.AnyAsync(
            u => u.Email == email.ToLowerInvariant() && !u.IsDeleted && (excludeId == null || u.Id != excludeId),
            cancellationToken);

    public async Task<(List<User> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        bool?   isActive,
        int     page,
        int     pageSize,
        string  sortBy,
        string  sortOrder,
        CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = context.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.ToLowerInvariant();
            query = query.Where(u =>
                u.Email.Contains(term)       ||
                u.FullName.Contains(term)    ||
                u.Identification.Contains(term));
        }

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        int totalCount = await query.CountAsync(cancellationToken);

        query = (sortBy.ToLowerInvariant(), sortOrder.ToLowerInvariant()) switch
        {
            ("email",    "desc") => query.OrderByDescending(u => u.Email),
            ("email",    _)      => query.OrderBy(u => u.Email),
            ("position", "desc") => query.OrderByDescending(u => u.Position),
            ("position", _)      => query.OrderBy(u => u.Position),
            (_,          "desc") => query.OrderByDescending(u => u.FullName),
            _                    => query.OrderBy(u => u.FullName),
        };

        List<User> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);


        

        return (items, totalCount);
    }
}
