using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.ArchiveProject;

public sealed record ArchiveProjectCommand(Guid Id) : ICommand;
