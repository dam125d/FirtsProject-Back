using Intap.FirstProject.Domain.Empleados;

namespace Intap.FirstProject.Application.Contracts;

public interface IEmpleadoWriteRepository
{
    Task<Empleado?> GetByIdAsync(int id, CancellationToken ct = default);
    void Add(Empleado empleado);
}
