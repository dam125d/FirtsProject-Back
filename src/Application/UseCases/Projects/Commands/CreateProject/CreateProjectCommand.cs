using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string       Name,
    string       Client,
    string?      Description,
    List<string> Scope,
    string       StartDate,
    string?      EndDate
) : ICommand<Guid>;
