using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(string Name, string Description) : ICommand<Guid>;
