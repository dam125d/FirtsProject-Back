namespace Intap.FirstProject.Application.UseCases.Users.DTOs;

public sealed record UserDto(
    Guid   Id,
    string Email,
    string FullName,
    string Identification,
    string Phone,
    string Position,
    Guid   RoleId,
    bool   IsActive,
    bool   IsLocked
);
