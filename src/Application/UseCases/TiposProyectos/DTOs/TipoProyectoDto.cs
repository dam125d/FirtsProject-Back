namespace Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;

public sealed record TipoProyectoDto(
    Guid       Id,
    string     Nombre,
    string?    Descripcion,
    bool       Estado,
    DateTime   CreatedAt,
    DateTime?  UpdatedAt);
