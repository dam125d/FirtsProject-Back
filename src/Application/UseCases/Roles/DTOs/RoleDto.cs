namespace Intap.FirstProject.Application.UseCases.Roles.DTOs;

public sealed record RoleDto(
    Guid   Id,
    string Name,
    string Description,
    bool   IsActive,
    bool   IsSystem,
    int    PermissionCount);
