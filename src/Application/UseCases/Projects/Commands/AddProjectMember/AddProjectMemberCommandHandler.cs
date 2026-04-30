using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Projects.Errors;
using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.AddProjectMember;

internal sealed class AddProjectMemberCommandHandler(
    IProjectWriteRepository projectWriteRepository,
    IUnitOfWork             unitOfWork
) : ICommandHandler<AddProjectMemberCommand>
{
    public async Task<Result> Handle(AddProjectMemberCommand command, CancellationToken cancellationToken)
    {
        Project? project = await projectWriteRepository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        if (!Enum.TryParse<AccessLevel>(command.AccessLevel, ignoreCase: true, out AccessLevel accessLevel))
            return Result.Failure(ProjectErrors.InvalidAccessLevel);

        project.AddMember(command.UserId, accessLevel);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
