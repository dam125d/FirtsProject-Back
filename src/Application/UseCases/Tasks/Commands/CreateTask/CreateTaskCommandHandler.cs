using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Tasks.Errors;
using Intap.FirstProject.Domain.Tasks;
using DomainTaskStatus = Intap.FirstProject.Domain.Tasks.TaskStatus;

namespace Intap.FirstProject.Application.UseCases.Tasks.Commands.CreateTask;

internal sealed class CreateTaskCommandHandler(
    ITaskWriteRepository taskWriteRepository,
    ICurrentUser         currentUser,
    IUnitOfWork          unitOfWork
) : ICommandHandler<CreateTaskCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        Guid userId = command.UserId ?? currentUser.UserId
            ?? throw new InvalidOperationException("No user identity available to create the task.");

        if (!DateOnly.TryParse(command.Date, out DateOnly date))
            return Result.Failure<Guid>(TaskErrors.InvalidDate);

        if (!TimeOnly.TryParse(command.StartTime, out TimeOnly startTime))
            return Result.Failure<Guid>(TaskErrors.InvalidDate);

        if (!TimeOnly.TryParse(command.EndTime, out TimeOnly endTime))
            return Result.Failure<Guid>(TaskErrors.InvalidDate);

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

        TaskEntry task = TaskEntry.Create(command.ProjectId, userId, data);

        await taskWriteRepository.AddAsync(task, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(task.Id);
    }
}
