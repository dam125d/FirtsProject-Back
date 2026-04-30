using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string FullName,
    string Identification,
    string Phone,
    string Position,
    Guid   RoleId
) : ICommand<Guid>;
