using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

internal sealed class TaskWriteRepository(AppDbContext context) : ITaskWriteRepository
{
    public Task<TaskEntry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

    public async Task AddAsync(TaskEntry task, CancellationToken ct = default) =>
        await context.Tasks.AddAsync(task, ct);

    public void Update(TaskEntry task) =>
        context.Tasks.Update(task);
}
