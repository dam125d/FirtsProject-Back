using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid    Id,
    string  Email,
    string? Password,
    string  FullName,
    string  Identification,
    string  Phone,
    string  Position,
    Guid    RoleId
) : ICommand;
