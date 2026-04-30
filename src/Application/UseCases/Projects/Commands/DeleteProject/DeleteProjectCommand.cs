using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.DeleteProject;

public sealed record DeleteProjectCommand(Guid Id) : ICommand;
