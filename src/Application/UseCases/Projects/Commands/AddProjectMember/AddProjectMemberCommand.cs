using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.AddProjectMember;

public sealed record AddProjectMemberCommand(
    Guid   ProjectId,
    Guid   UserId,
    string AccessLevel
) : ICommand;
