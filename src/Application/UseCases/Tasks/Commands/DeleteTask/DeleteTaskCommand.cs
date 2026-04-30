using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id) : ICommand;
