using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.CreateTipoProyecto;

public sealed record CreateTipoProyectoCommand(
    string  Nombre,
    string? Descripcion
) : ICommand<Guid>;
