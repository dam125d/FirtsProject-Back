using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.Categorias.Commands.CreateCategoria;

public sealed class CreateCategoriaCommandValidator : AbstractValidator<CreateCategoriaCommand>
{
    public CreateCategoriaCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .When(x => x.Descripcion is not null);
    }
}
