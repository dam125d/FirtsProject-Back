namespace Intap.FirstProject.Application.UseCases.Auth.DTOs;

public sealed record LoginResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    IReadOnlyList<string> Permissions
);
