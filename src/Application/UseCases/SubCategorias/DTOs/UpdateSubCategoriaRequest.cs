namespace Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;

public sealed record UpdateSubCategoriaRequest(
    string  Nombre,
    string? Descripcion,
    Guid    CategoriaId,
    bool    Estado);
