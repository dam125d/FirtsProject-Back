using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.TiposProyectos;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class TipoProyectoWriteRepository(AppDbContext context) : ITipoProyectoWriteRepository
{
    public Task<TipoProyecto?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.TiposProyecto.FirstOrDefaultAsync(t => t.Id == id, ct);

    public void Add(TipoProyecto tipoProyecto) =>
        context.TiposProyecto.Add(tipoProyecto);
}
