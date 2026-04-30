using Intap.FirstProject.Domain.Tasks;

namespace Intap.FirstProject.Application.Contracts;

public interface ITaskWriteRepository
{
    Task<TaskEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task             AddAsync(TaskEntry task, CancellationToken ct = default);
    void             Update(TaskEntry task);
}
