using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Domain.SubCategorias;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class SubCategoriaReadRepository(AppDbContext context) : ISubCategoriaReadRepository
{
    public Task<List<SubCategoria>> GetAllAsync(bool soloActivas, Guid? categoriaId, CancellationToken ct = default)
    {
        IQueryable<SubCategoria> query = context.SubCategorias;

        if (soloActivas)
            query = query.Where(s => s.Estado);

        if (categoriaId.HasValue)
            query = query.Where(s => s.CategoriaId == categoriaId.Value);

        return query
            .OrderBy(s => s.Nombre)
            .ToListAsync(ct);
    }

    public Task<SubCategoria?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.SubCategorias.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<bool> ExistsByNombreAndCategoriaAsync(string nombre, Guid categoriaId, CancellationToken ct = default) =>
        context.SubCategorias.AnyAsync(
            s => s.Estado && s.CategoriaId == categoriaId && s.Nombre.ToLower() == nombre.ToLower(), ct);

    public Task<bool> ExistsByNombreAndCategoriaExcludingIdAsync(string nombre, Guid categoriaId, Guid excludeId, CancellationToken ct = default) =>
        context.SubCategorias.AnyAsync(
            s => s.Estado && s.CategoriaId == categoriaId && s.Id != excludeId && s.Nombre.ToLower() == nombre.ToLower(), ct);
}
