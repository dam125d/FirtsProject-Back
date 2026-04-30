using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Domain.TiposTareas;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class TipoTareaReadRepository(AppDbContext context) : ITipoTareaReadRepository
{
    public async Task<(List<TipoTarea> Items, int Total)> GetAllPagedAsync(
        int               page,
        int               pageSize,
        bool?             estado,
        string?           nombre,
        CancellationToken ct = default)
    {
        IQueryable<TipoTarea> query = context.TiposTarea;

        if (estado.HasValue)
            query = query.Where(t => t.Estado == estado.Value);

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(t => EF.Functions.ILike(t.Nombre, $"%{nombre}%"));

        int total = await query.CountAsync(ct);

        List<TipoTarea> items = await query
            .OrderBy(t => t.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<TipoTarea?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.TiposTarea.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<bool> ExistsActiveByNombreAsync(string nombre, CancellationToken ct = default) =>
        context.TiposTarea.AnyAsync(
            t => t.Estado && t.Nombre.ToLower() == nombre.ToLower(),
            ct);

    public Task<bool> ExistsActiveByNombreExcludingIdAsync(
        string            nombre,
        Guid              excludeId,
        CancellationToken ct = default) =>
        context.TiposTarea.AnyAsync(
            t => t.Estado && t.Nombre.ToLower() == nombre.ToLower() && t.Id != excludeId,
            ct);
}
