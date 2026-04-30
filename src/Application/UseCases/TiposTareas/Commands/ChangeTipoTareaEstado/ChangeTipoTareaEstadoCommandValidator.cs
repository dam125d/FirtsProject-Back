using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Commands.ChangeTipoTareaEstado;

public sealed class ChangeTipoTareaEstadoCommandValidator : AbstractValidator<ChangeTipoTareaEstadoCommand>
{
    public ChangeTipoTareaEstadoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
