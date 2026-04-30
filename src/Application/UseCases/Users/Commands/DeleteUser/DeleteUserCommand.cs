using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : ICommand;
