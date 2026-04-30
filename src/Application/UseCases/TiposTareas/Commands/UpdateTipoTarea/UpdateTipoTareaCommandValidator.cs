using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Commands.UpdateTipoTarea;

public sealed class UpdateTipoTareaCommandValidator : AbstractValidator<UpdateTipoTareaCommand>
{
    public UpdateTipoTareaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(150)
            .Must(n => !ContainsHtmlTags(n))
            .WithMessage("'Nombre' must not contain HTML tags.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .Must(d => d is null || !ContainsHtmlTags(d))
            .WithMessage("'Descripcion' must not contain HTML tags.")
            .When(x => x.Descripcion is not null);
    }

    private static bool ContainsHtmlTags(string value) =>
        value.Contains('<') || value.Contains('>');
}
