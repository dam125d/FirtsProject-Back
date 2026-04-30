namespace Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;

public sealed record SubCategoriaDto(
    Guid      Id,
    string    Nombre,
    string?   Descripcion,
    Guid      CategoriaId,
    bool      Estado,
    DateTime  CreatedAt,
    DateTime? UpdatedAt);
