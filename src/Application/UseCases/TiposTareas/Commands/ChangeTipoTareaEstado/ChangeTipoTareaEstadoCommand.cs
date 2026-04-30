using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Commands.ChangeTipoTareaEstado;

public sealed record ChangeTipoTareaEstadoCommand(
    Guid Id,
    bool Estado
) : ICommand<TipoTareaEstadoDto>;
