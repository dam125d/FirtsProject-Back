using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Roles.Commands.AssignRolePermissions;

public sealed record AssignRolePermissionsCommand(
    Guid                  RoleId,
    IReadOnlyList<string> PermissionCodes) : ICommand;
