namespace Intap.FirstProject.Application.UseCases.Roles.DTOs;

public sealed record RoleDetailDto(
    Guid                   Id,
    string                 Name,
    string                 Description,
    bool                   IsActive,
    bool                   IsSystem,
    IReadOnlyList<string>  PermissionCodes);
