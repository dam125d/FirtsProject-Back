using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.ToggleUserStatus;

public sealed record ToggleUserStatusCommand(Guid Id) : ICommand;
