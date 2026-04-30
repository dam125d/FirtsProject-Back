namespace Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;

public sealed record TipoTareaDto(
    Guid      Id,
    string    Nombre,
    string?   Descripcion,
    bool      Estado,
    DateTime  CreatedAt,
    DateTime? UpdatedAt);
