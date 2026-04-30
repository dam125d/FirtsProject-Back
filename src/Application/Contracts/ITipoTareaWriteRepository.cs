using Intap.FirstProject.Domain.TiposTareas;

namespace Intap.FirstProject.Application.Contracts;

public interface ITipoTareaWriteRepository
{
    Task<TipoTarea?> GetByIdAsync(Guid id, CancellationToken ct = default);
    void Add(TipoTarea tipoTarea);
}
