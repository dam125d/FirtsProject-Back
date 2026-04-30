using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid    ProjectId,
    Guid?   UserId,
    string  Description,
    string  Date,
    string  StartTime,
    string  EndTime,
    string  Duration,
    string  Status,
    string? Observations
) : ICommand<Guid>;
