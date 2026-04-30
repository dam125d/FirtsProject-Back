using Intap.FirstProject.Domain.Empleados;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface IEmpleadoReadRepository
{
    Task<List<Empleado>> GetAllAsync(bool soloActivos, CancellationToken ct = default);
    Task<Empleado?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsByIdentificacionAsync(string identificacion, CancellationToken ct = default);
    Task<bool> ExistsByIdentificacionExcludingIdAsync(string identificacion, int excludeId, CancellationToken ct = default);
    Task<bool> ExistsByCorreoAsync(string correo, CancellationToken ct = default);
    Task<bool> ExistsByCorreoExcludingIdAsync(string correo, int excludeId, CancellationToken ct = default);
}
