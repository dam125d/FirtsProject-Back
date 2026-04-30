using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid         Id,
    string       Name,
    string       Client,
    string?      Description,
    List<string> Scope,
    string       StartDate,
    string?      EndDate
) : ICommand;
