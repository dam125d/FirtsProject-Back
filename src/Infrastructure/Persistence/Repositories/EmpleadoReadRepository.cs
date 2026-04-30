using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Domain.Empleados;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class EmpleadoReadRepository(AppDbContext context) : IEmpleadoReadRepository
{
    public Task<List<Empleado>> GetAllAsync(bool soloActivos, CancellationToken ct = default)
    {
        IQueryable<Empleado> query = context.Empleados;

        if (soloActivos)
            query = query.Where(e => e.Estado);

        return query
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombres)
            .ToListAsync(ct);
    }

    public Task<Empleado?> GetByIdAsync(int id, CancellationToken ct = default) =>
        context.Empleados.FirstOrDefaultAsync(e => e.Id == id && e.Estado, ct);

    public Task<bool> ExistsByIdentificacionAsync(string identificacion, CancellationToken ct = default) =>
        context.Empleados.AnyAsync(
            e => e.Identificacion.ToLower() == identificacion.ToLower(), ct);

    public Task<bool> ExistsByIdentificacionExcludingIdAsync(
        string identificacion, int excludeId, CancellationToken ct = default) =>
        context.Empleados.AnyAsync(
            e => e.Identificacion.ToLower() == identificacion.ToLower() && e.Id != excludeId, ct);

    public Task<bool> ExistsByCorreoAsync(string correo, CancellationToken ct = default) =>
        context.Empleados.AnyAsync(
            e => e.Correo.ToLower() == correo.ToLower(), ct);

    public Task<bool> ExistsByCorreoExcludingIdAsync(
        string correo, int excludeId, CancellationToken ct = default) =>
        context.Empleados.AnyAsync(
            e => e.Correo.ToLower() == correo.ToLower() && e.Id != excludeId, ct);
}
