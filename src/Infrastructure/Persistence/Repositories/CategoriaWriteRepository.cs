using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class CategoriaWriteRepository(AppDbContext context) : ICategoriaWriteRepository
{
    public Task<Categoria?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Categorias.FirstOrDefaultAsync(c => c.Id == id, ct);

    public void Add(Categoria categoria) =>
        context.Categorias.Add(categoria);
}
