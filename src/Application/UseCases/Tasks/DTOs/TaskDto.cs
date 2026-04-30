namespace Intap.FirstProject.Application.UseCases.Tasks.DTOs;

public sealed record TaskDto(
    Guid    Id,
    Guid    ProjectId,
    string  Project,
    Guid    UserId,
    string  Employee,
    string  Description,
    string  Date,
    string  StartTime,
    string  EndTime,
    string  Duration,
    string  Status,
    string? Observations
);
