using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Roles.Commands.UpdateRole;

public sealed record UpdateRoleCommand(Guid Id, string Name, string Description) : ICommand;
