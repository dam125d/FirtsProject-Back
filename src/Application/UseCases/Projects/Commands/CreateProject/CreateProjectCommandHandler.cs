using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Domain.Projects;

namespace Intap.FirstProject.Application.UseCases.Projects.Commands.CreateProject;

internal sealed class CreateProjectCommandHandler(
    IProjectWriteRepository projectWriteRepository,
    IUnitOfWork             unitOfWork
) : ICommandHandler<CreateProjectCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        DateOnly startDate = DateOnly.Parse(command.StartDate);
        DateOnly? endDate  = command.EndDate is not null ? DateOnly.Parse(command.EndDate) : null;

        Project project = Project.Create(
            command.Name,
            command.Client,
            command.Description,
            command.Scope,
            startDate,
            endDate);

        await projectWriteRepository.AddAsync(project, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(project.Id);
    }
}
