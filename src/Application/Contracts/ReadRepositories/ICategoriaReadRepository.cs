using Intap.FirstProject.Domain.Categorias;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface ICategoriaReadRepository
{
    Task<List<Categoria>> GetAllAsync(bool soloActivas, CancellationToken ct = default);
    Task<Categoria?>      GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool>            ExistsByNombreAsync(string nombre, CancellationToken ct = default);
    Task<bool>            ExistsByNombreExcludingIdAsync(string nombre, Guid excludeId, CancellationToken ct = default);
    Task<bool>            HasActiveProjectsAsync(Guid categoriaId, CancellationToken ct = default);
}
