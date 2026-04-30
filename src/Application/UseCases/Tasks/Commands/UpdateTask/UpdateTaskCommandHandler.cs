using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Tasks.Errors;
using Intap.FirstProject.Domain.Tasks;
using DomainTaskStatus = Intap.FirstProject.Domain.Tasks.TaskStatus;

namespace Intap.FirstProject.Application.UseCases.Tasks.Commands.UpdateTask;

internal sealed class UpdateTaskCommandHandler(
    ITaskWriteRepository taskWriteRepository,
    IUnitOfWork          unitOfWork
) : ICommandHandler<UpdateTaskCommand>
{
    public async Task<Result> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        TaskEntry? task = await taskWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (task is null)
            return Result.Failure(TaskErrors.NotFound);

        if (!DateOnly.TryParse(command.Date, out DateOnly date))
            return Result.Failure(TaskErrors.InvalidDate);

        if (!TimeOnly.TryParse(command.StartTime, out TimeOnly startTime))
            return Result.Failure(TaskErrors.InvalidDate);

        if (!TimeOnly.TryParse(command.EndTime, out TimeOnly endTime))
            return Result.Failure(TaskErrors.InvalidDate);

        DomainTaskStatus status = command.Status.ToLowerInvariant() switch
        {
            "in-progress" => DomainTaskStatus.InProgress,
            "pending"     => DomainTaskStatus.Pending,
            "completed"   => DomainTaskStatus.Completed,
            "blocked"     => DomainTaskStatus.Blocked,
            _             => DomainTaskStatus.Pending,
        };

        TaskEntryData data = new(
            command.Description,
            date,
            startTime,
            endTime,
            command.Duration,
            status,
            command.Observations);

        task.Update(data);
        taskWriteRepository.Update(task);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
