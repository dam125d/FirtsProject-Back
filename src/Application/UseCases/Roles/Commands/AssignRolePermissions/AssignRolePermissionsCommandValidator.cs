using FluentValidation;

namespace Intap.FirstProject.Application.UseCases.Roles.Commands.AssignRolePermissions;

public sealed class AssignRolePermissionsCommandValidator : AbstractValidator<AssignRolePermissionsCommand>
{
    public AssignRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.PermissionCodes)
            .NotNull();
    }
}
