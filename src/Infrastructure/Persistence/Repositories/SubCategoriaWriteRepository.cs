using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.SubCategorias;
using Microsoft.EntityFrameworkCore;

namespace Intap.FirstProject.Infrastructure.Persistence.Repositories;

public sealed class SubCategoriaWriteRepository(AppDbContext context) : ISubCategoriaWriteRepository
{
    public Task<SubCategoria?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.SubCategorias.FirstOrDefaultAsync(s => s.Id == id, ct);

    public void Add(SubCategoria subCategoria) =>
        context.SubCategorias.Add(subCategoria);
}
