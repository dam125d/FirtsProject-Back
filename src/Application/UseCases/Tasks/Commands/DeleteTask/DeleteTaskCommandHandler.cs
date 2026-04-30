using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Tasks.Errors;
using Intap.FirstProject.Domain.Tasks;

namespace Intap.FirstProject.Application.UseCases.Tasks.Commands.DeleteTask;

internal sealed class DeleteTaskCommandHandler(
    ITaskWriteRepository taskWriteRepository,
    IUnitOfWork          unitOfWork
) : ICommandHandler<DeleteTaskCommand>
{
    public async Task<Result> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        TaskEntry? task = await taskWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (task is null)
            return Result.Failure(TaskErrors.NotFound);

        task.Delete();
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
