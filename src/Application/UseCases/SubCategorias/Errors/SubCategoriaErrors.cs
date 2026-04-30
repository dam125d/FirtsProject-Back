using Intap.FirstProject.Application.Abstractions.Results;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Errors;

public static class SubCategoriaErrors
{
    public static readonly ErrorResult NotFound =
        new("SUBCATEGORIA_NOT_FOUND", "SubCategory not found.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult NombreRequerido =
        new("SUBCATEGORIA_NOMBRE_REQUERIDO", "SubCategory name is required.", ErrorTypeResult.Validation);

    public static readonly ErrorResult CategoriaRequerida =
        new("SUBCATEGORIA_CATEGORIA_REQUERIDA", "Category is required.", ErrorTypeResult.Validation);

    public static readonly ErrorResult NombreDuplicado =
        new("SUBCATEGORIA_NOMBRE_DUPLICADO", "SubCategory name already exists in this category.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult CategoriaNaoEncontrada =
        new("SUBCATEGORIA_CATEGORIA_NOT_FOUND", "Parent category not found.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult CategoriaInactiva =
        new("SUBCATEGORIA_CATEGORIA_INACTIVA", "Parent category is inactive.", ErrorTypeResult.Unprocessable);
}
