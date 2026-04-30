using Intap.FirstProject.Domain.SubCategorias;

namespace Intap.FirstProject.Application.Contracts;

public interface ISubCategoriaWriteRepository
{
    Task<SubCategoria?> GetByIdAsync(Guid id, CancellationToken ct = default);
    void Add(SubCategoria subCategoria);
}
