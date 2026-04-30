namespace Intap.FirstProject.Domain.Permissions;

public sealed class Permission
{
    public string Code   { get; private set; } = null!;
    public string Name   { get; private set; } = null!;
    public string Module { get; private set; } = string.Empty;

    private Permission() { }

    public static Permission Create(string code, string name, string module = "")
    {
        return new Permission { Code = code, Name = name, Module = module };
    }
}
