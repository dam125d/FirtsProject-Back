using Intap.FirstProject.Application.Abstractions.Results;

namespace Intap.FirstProject.Application.UseCases.Roles.Errors;

public static class RoleErrors
{
    public static readonly ErrorResult NotFound =
        new("Role.NotFound", "Role was not found.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult AlreadyExists =
        new("Role.AlreadyExists", "A role with this name already exists.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult SystemRoleCannotBeModified =
        new("Role.SystemRoleCannotBeModified", "System roles cannot be modified or deleted.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult RoleInUse =
        new("Role.RoleInUse", "Role has users assigned and cannot be deleted.", ErrorTypeResult.Conflict);
}
