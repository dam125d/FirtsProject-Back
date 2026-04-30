using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.Empleados;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class EmpleadoWriteRepository(AppDbContext context) : IEmpleadoWriteRepository
{
    public Task<Empleado?> GetByIdAsync(int id, CancellationToken ct = default) =>
        context.Empleados.FirstOrDefaultAsync(e => e.Id == id, ct);

    public void Add(Empleado empleado) =>
        context.Empleados.Add(empleado);
}
