using Intap.FirstProject.Domain.TiposProyectos;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface ITipoProyectoReadRepository
{
    Task<(List<TipoProyecto> Items, int Total)> GetAllPagedAsync(
        int               page,
        int               pageSize,
        bool?             estado,
        string?           nombre,
        CancellationToken ct = default);

    Task<TipoProyecto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsActiveByNombreAsync(
        string            nombre,
        CancellationToken ct = default);

    Task<bool> ExistsActiveByNombreExcludingIdAsync(
        string            nombre,
        Guid              excludeId,
        CancellationToken ct = default);
}
