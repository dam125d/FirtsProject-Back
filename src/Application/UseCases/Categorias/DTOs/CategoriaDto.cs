namespace Intap.FirstProject.Application.UseCases.Categorias.DTOs;

public sealed record CategoriaDto(
    Guid      Id,
    string    Nombre,
    string?   Descripcion,
    bool      Estado,
    DateTime  CreatedAt,
    DateTime? UpdatedAt);
