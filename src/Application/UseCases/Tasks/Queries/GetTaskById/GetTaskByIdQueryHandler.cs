using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Tasks.DTOs;
using Intap.FirstProject.Application.UseCases.Tasks.Errors;
using Intap.FirstProject.Domain.Tasks;

namespace Intap.FirstProject.Application.UseCases.Tasks.Queries.GetTaskById;

internal sealed class GetTaskByIdQueryHandler(
    ITaskReadRepository taskReadRepository,
    IMapper             mapper
) : IQueryHandler<GetTaskByIdQuery, TaskDto>
{
    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        TaskEntry? task = await taskReadRepository.GetByIdAsync(query.Id, cancellationToken);
        if (task is null)
            return Result.Failure<TaskDto>(TaskErrors.NotFound);

        TaskDto dto = mapper.Map<TaskDto>(task);
        return Result.Success(dto);
    }
}
