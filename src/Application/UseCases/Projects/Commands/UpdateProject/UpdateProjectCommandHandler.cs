using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Projects.Errors;
using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.UpdateProject;

internal sealed class UpdateProjectCommandHandler(
    IProjectWriteRepository projectWriteRepository,
    IUnitOfWork             unitOfWork
) : ICommandHandler<UpdateProjectCommand>
{
    public async Task<Result> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        Project? project = await projectWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        DateOnly  startDate = DateOnly.Parse(command.StartDate);
        DateOnly? endDate   = command.EndDate is not null ? DateOnly.Parse(command.EndDate) : null;

        project.Update(command.Name, command.Client, command.Description, command.Scope, startDate, endDate);
        projectWriteRepository.Update(project);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
