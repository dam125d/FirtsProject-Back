using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Client)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .Must(BeAValidDate)
            .WithMessage("StartDate must be a valid date in yyyy-MM-dd format.");

        RuleFor(x => x.EndDate)
            .Must(BeAValidDate)
            .WithMessage("EndDate must be a valid date in yyyy-MM-dd format.")
            .When(x => x.EndDate is not null);
    }

    private static bool BeAValidDate(string? value) =>
        value is not null && DateOnly.TryParse(value, out _);
}
