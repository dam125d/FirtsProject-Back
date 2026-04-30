namespace Intap.FirstProject.Domain.Roles;

public sealed class RolePermission
{
    public Guid   RoleId         { get; private set; }
    public string PermissionCode { get; private set; } = null!;

    private RolePermission() { }

    public static RolePermission Create(Guid roleId, string permissionCode)
    {
        return new RolePermission { RoleId = roleId, PermissionCode = permissionCode };
    }
}
