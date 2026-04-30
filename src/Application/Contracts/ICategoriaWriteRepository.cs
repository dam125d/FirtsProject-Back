using Intap.FirstProject.Domain.Categorias;

namespace Intap.FirstProject.Application.Contracts;

public interface ICategoriaWriteRepository
{
    Task<Categoria?> GetByIdAsync(Guid id, CancellationToken ct = default);
    void Add(Categoria categoria);
}
