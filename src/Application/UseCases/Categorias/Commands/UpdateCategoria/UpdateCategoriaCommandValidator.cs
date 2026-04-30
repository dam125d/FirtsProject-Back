using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.Categorias.Commands.UpdateCategoria;

public sealed class UpdateCategoriaCommandValidator : AbstractValidator<UpdateCategoriaCommand>
{
    public UpdateCategoriaCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .When(x => x.Descripcion is not null);
    }
}
