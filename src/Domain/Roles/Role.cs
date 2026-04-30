using Intap.FirstProject.Domain.Common;

namespace Intap.FirstProject.Domain.Roles;

public sealed class Role : BaseEntity
{
    private readonly List<RolePermission> _permissions = new();

    private Role() { }

    public string Name        { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool   IsActive    { get; private set; } = true;
    public bool   IsSystem    { get; private set; }          // System roles cannot be deleted

    public IReadOnlyList<RolePermission> Permissions => _permissions.AsReadOnly();

    public static Role Create(string name, string description, bool isSystem = false)
    {
        return new Role
        {
            Name        = name.Trim(),
            Description = description.Trim(),
            IsActive    = true,
            IsSystem    = isSystem,
        };
    }

    public void Update(string name, string description)
    {
        Name        = name.Trim();
        Description = description.Trim();
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void AssignPermissions(IEnumerable<string> permissionCodes)
    {
        _permissions.Clear();
        foreach (string code in permissionCodes)
        {
            _permissions.Add(RolePermission.Create(Id, code));
        }
        SetUpdatedAt();
    }

    public IReadOnlyList<string> GetPermissionCodes()
    {
        return _permissions.Select(p => p.PermissionCode).ToList().AsReadOnly();
    }
}
