using Intap.FirstProject.Application.Abstractions.Results;

namespace Intap.FirstProject.Application.UseCases.Categorias.Errors;

public static class CategoriaErrors
{
    public static readonly ErrorResult NotFound =
        new("Categoria.NotFound", "Category was not found.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult NombreDuplicado =
        new("Categoria.NombreDuplicado", "A category with this name already exists.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult EnUso =
        new("Categoria.EnUso", "Category is assigned to active projects.", ErrorTypeResult.Conflict);
}
