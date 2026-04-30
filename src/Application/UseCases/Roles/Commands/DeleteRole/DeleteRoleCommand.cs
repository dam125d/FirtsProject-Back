using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Roles.Commands.DeleteRole;

public sealed record DeleteRoleCommand(Guid Id) : ICommand;
