namespace Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;

public sealed record TipoProyectoEstadoDto(
    Guid      Id,
    bool      Estado,
    DateTime  UpdatedAt);
