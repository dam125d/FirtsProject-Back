using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.Empleados.Commands.CreateEmpleado;

public sealed class CreateEmpleadoCommandValidator : AbstractValidator<CreateEmpleadoCommand>
{
    public CreateEmpleadoCommandValidator()
    {
        RuleFor(x => x.Identificacion)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Nombres)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Apellidos)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Correo)
            .NotEmpty()
            .MaximumLength(150)
            .EmailAddress();

        RuleFor(x => x.Telefono)
            .MaximumLength(20)
            .When(x => x.Telefono is not null);

        RuleFor(x => x.Cargo)
            .NotEmpty()
            .MaximumLength(100);
    }
}
