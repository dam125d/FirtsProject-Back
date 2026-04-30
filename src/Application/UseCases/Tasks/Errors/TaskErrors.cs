using Intap.FirstProject.Application.Abstractions.Results;

namespace Intap.FirstProject.Application.UseCases.Tasks.Errors;

public static class TaskErrors
{
    public static readonly ErrorResult NotFound =
        new("Task.NotFound", "Task was not found.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult InvalidStatus =
        new("Task.InvalidStatus", "The provided task status is not valid.", ErrorTypeResult.Validation);

    public static readonly ErrorResult InvalidDate =
        new("Task.InvalidDate", "The provided date is not valid.", ErrorTypeResult.Validation);
}
