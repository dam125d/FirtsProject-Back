using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Commands.CreateSubCategoria;

public sealed class CreateSubCategoriaCommandValidator : AbstractValidator<CreateSubCategoriaCommand>
{
    public CreateSubCategoriaCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .When(x => x.Descripcion is not null);

        RuleFor(x => x.CategoriaId)
            .NotEmpty();
    }
}
