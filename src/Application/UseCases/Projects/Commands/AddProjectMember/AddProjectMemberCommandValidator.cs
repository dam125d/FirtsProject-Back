using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.AddProjectMember;

public sealed class AddProjectMemberCommandValidator : AbstractValidator<AddProjectMemberCommand>
{
    private static readonly string[] ValidAccessLevels = ["ReadOnly", "Contributor", "Admin"];

    public AddProjectMemberCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.AccessLevel)
            .NotEmpty()
            .Must(v => ValidAccessLevels.Contains(v, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"AccessLevel must be one of: {string.Join(", ", ValidAccessLevels)}.");
    }
}
