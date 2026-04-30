using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Commands.UpdateSubCategoria;

public sealed class UpdateSubCategoriaCommandValidator : AbstractValidator<UpdateSubCategoriaCommand>
{
    public UpdateSubCategoriaCommandValidator()
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
