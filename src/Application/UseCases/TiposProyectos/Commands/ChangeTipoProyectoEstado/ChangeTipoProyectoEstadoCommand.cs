using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.ChangeTipoProyectoEstado;

public sealed record ChangeTipoProyectoEstadoCommand(
    Guid Id,
    bool Estado
) : ICommand<TipoProyectoEstadoDto>;
