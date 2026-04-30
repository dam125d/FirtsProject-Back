using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using DomainTaskStatus = Intap.FirstProject.Domain.Tasks.TaskStatus;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

internal sealed class TaskReadRepository(AppDbContext context) : ITaskReadRepository
{
    public Task<TaskEntry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Tasks
            .Include(t => t.Project)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

    public async Task<(List<TaskEntry> Items, int TotalCount)> GetPagedAsync(
        Guid?  projectId,
        Guid?  userId,
        string? status,
        string? date,
        int    page,
        int    pageSize,
        CancellationToken ct = default)
    {
        IQueryable<TaskEntry> query = context.Tasks
            .Include(t => t.Project)
            .Include(t => t.User)
            .Where(t => !t.IsDeleted);

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(status))
        {
            DomainTaskStatus taskStatus = status.ToLowerInvariant() switch
            {
                "in-progress" => DomainTaskStatus.InProgress,
                "pending"     => DomainTaskStatus.Pending,
                "completed"   => DomainTaskStatus.Completed,
                "blocked"     => DomainTaskStatus.Blocked,
                _             => DomainTaskStatus.Pending,
            };
            query = query.Where(t => t.Status == taskStatus);
        }

        if (!string.IsNullOrWhiteSpace(date) && DateOnly.TryParse(date, out DateOnly parsedDate))
            query = query.Where(t => t.Date == parsedDate);

        int totalCount = await query.CountAsync(ct);

        List<TaskEntry> items = await query
            .OrderByDescending(t => t.Date)
            .ThenBy(t => t.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
