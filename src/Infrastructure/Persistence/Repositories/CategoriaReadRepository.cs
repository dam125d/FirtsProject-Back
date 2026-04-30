using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Domain.Categorias;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class CategoriaReadRepository(AppDbContext context) : ICategoriaReadRepository
{
    public Task<List<Categoria>> GetAllAsync(bool soloActivas, CancellationToken ct = default)
    {
        IQueryable<Categoria> query = context.Categorias;

        if (soloActivas)
            query = query.Where(c => c.Estado);

        return query
            .OrderBy(c => c.Nombre)
            .ToListAsync(ct);
    }

    public Task<Categoria?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Categorias.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<bool> ExistsByNombreAsync(string nombre, CancellationToken ct = default) =>
        context.Categorias.AnyAsync(
            c => c.Nombre.ToLower() == nombre.ToLower(), ct);

    public Task<bool> ExistsByNombreExcludingIdAsync(string nombre, Guid excludeId, CancellationToken ct = default) =>
        context.Categorias.AnyAsync(
            c => c.Nombre.ToLower() == nombre.ToLower() && c.Id != excludeId, ct);

    public Task<bool> HasActiveProjectsAsync(Guid categoriaId, CancellationToken ct = default) =>
        context.Projects.AnyAsync(
            p => p.CategoriaId == categoriaId && !p.IsDeleted, ct);
}
