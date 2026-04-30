namespace Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;

public sealed record CreateSubCategoriaRequest(
    string  Nombre,
    string? Descripcion,
    Guid    CategoriaId);
