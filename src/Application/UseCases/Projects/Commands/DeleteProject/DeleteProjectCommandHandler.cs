using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Projects.Errors;
using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.DeleteProject;

internal sealed class DeleteProjectCommandHandler(
    IProjectWriteRepository projectWriteRepository,
    IUnitOfWork             unitOfWork
) : ICommandHandler<DeleteProjectCommand>
{
    public async Task<Result> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        Project? project = await projectWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        project.Delete();
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
