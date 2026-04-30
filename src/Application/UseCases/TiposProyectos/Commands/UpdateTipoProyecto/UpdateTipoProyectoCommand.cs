using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.UpdateTipoProyecto;

public sealed record UpdateTipoProyectoCommand(
    Guid    Id,
    string  Nombre,
    string? Descripcion
) : ICommand<Guid>;
