namespace Intap.FirstProject.API.Requests.Roles;

public sealed record AssignRolePermissionsRequest(IReadOnlyList<string> PermissionCodes);
