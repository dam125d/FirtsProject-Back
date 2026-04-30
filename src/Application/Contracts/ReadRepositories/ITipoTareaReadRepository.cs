using Intap.FirstProject.Domain.TiposTareas;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface ITipoTareaReadRepository
{
    Task<(List<TipoTarea> Items, int Total)> GetAllPagedAsync(
        int               page,
        int               pageSize,
        bool?             estado,
        string?           nombre,
        CancellationToken ct = default);

    Task<TipoTarea?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsActiveByNombreAsync(
        string            nombre,
        CancellationToken ct = default);

    Task<bool> ExistsActiveByNombreExcludingIdAsync(
        string            nombre,
        Guid              excludeId,
        CancellationToken ct = default);
}
