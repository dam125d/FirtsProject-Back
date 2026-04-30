using Intap.FirstProject.Domain.SubCategorias;

namespace Intap.FirstProject.Application.Contracts.ReadRepositories;

public interface ISubCategoriaReadRepository
{
    Task<List<SubCategoria>> GetAllAsync(bool soloActivas, Guid? categoriaId, CancellationToken ct = default);
    Task<SubCategoria?>      GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool>               ExistsByNombreAndCategoriaAsync(string nombre, Guid categoriaId, CancellationToken ct = default);
    Task<bool>               ExistsByNombreAndCategoriaExcludingIdAsync(string nombre, Guid categoriaId, Guid excludeId, CancellationToken ct = default);
}
