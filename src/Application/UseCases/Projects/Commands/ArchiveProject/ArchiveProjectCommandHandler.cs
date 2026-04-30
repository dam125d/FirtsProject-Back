using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Projects.Errors;
using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.ArchiveProject;

internal sealed class ArchiveProjectCommandHandler(
    IProjectWriteRepository projectWriteRepository,
    IUnitOfWork             unitOfWork
) : ICommandHandler<ArchiveProjectCommand>
{
    public async Task<Result> Handle(ArchiveProjectCommand command, CancellationToken cancellationToken)
    {
        Project? project = await projectWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        project.Archive();
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
