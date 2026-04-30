using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.TiposTareas;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class TipoTareaWriteRepository(AppDbContext context) : ITipoTareaWriteRepository
{
    public Task<TipoTarea?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.TiposTarea.FirstOrDefaultAsync(t => t.Id == id, ct);

    public void Add(TipoTarea tipoTarea) =>
        context.TiposTarea.Add(tipoTarea);
}
