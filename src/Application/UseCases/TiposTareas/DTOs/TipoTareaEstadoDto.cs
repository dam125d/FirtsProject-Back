namespace Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;

public sealed record TipoTareaEstadoDto(
    Guid     Id,
    bool     Estado,
    DateTime UpdatedAt);
