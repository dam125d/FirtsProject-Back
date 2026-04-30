using Intap.FirstProject.Domain.Tasks;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface ITaskReadRepository
{
    Task<TaskEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(List<TaskEntry> Items, int TotalCount)> GetPagedAsync(
        Guid?  projectId,
        Guid?  userId,
        string? status,
        string? date,
        int    page,
        int    pageSize,
        CancellationToken ct = default);
}
