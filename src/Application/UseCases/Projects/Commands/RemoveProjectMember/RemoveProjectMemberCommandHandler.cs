using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Projects.Errors;
using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.RemoveProjectMember;

internal sealed class RemoveProjectMemberCommandHandler(
    IProjectWriteRepository projectWriteRepository,
    IUnitOfWork             unitOfWork
) : ICommandHandler<RemoveProjectMemberCommand>
{
    public async Task<Result> Handle(RemoveProjectMemberCommand command, CancellationToken cancellationToken)
    {
        Project? project = await projectWriteRepository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        project.RemoveMember(command.MemberId);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
