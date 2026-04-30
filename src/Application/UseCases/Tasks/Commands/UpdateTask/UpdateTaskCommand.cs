using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid    Id,
    string  Description,
    string  Date,
    string  StartTime,
    string  EndTime,
    string  Duration,
    string  Status,
    string? Observations
) : ICommand;
