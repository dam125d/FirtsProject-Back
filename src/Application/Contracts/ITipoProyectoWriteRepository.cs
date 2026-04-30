using Intap.FirstProject.Domain.TiposProyectos;

namespace Intap.FirstProject.Application.Contracts;

public interface ITipoProyectoWriteRepository
{
    Task<TipoProyecto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    void Add(TipoProyecto tipoProyecto);
}
