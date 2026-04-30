using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    private static readonly string[] ValidStatuses = ["pending", "in-progress", "completed", "blocked"];

    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Date)
            .NotEmpty()
            .Must(v => DateOnly.TryParse(v, out _))
            .WithMessage("Date must be a valid date in yyyy-MM-dd format.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .Must(v => TimeOnly.TryParse(v, out _))
            .WithMessage("StartTime must be a valid time.");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .Must(v => TimeOnly.TryParse(v, out _))
            .WithMessage("EndTime must be a valid time.");

        RuleFor(x => x.Duration)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(v => ValidStatuses.Contains(v.ToLowerInvariant()))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}.");

        RuleFor(x => x.Observations)
            .MaximumLength(1000)
            .When(x => x.Observations is not null);
    }
}
