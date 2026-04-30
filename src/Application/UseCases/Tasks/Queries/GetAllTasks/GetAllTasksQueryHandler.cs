using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Tasks.DTOs;
using Intap.FirstProject.Domain.Tasks;

namespace Intap.FirstProject.Application.UseCases.Tasks.Queries.GetAllTasks;

internal sealed class GetAllTasksQueryHandler(
    ITaskReadRepository taskReadRepository,
    IMapper             mapper
) : IQueryHandler<GetAllTasksQuery, PagedResult<TaskDto>>
{
    public async Task<Result<PagedResult<TaskDto>>> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        (List<TaskEntry> items, int totalCount) = await taskReadRepository.GetPagedAsync(
            query.ProjectId,
            query.UserId,
            query.Status,
            query.Date,
            query.Page,
            query.PageSize,
            cancellationToken);

        List<TaskDto> dtos            = mapper.Map<List<TaskDto>>(items);
        PagedResult<TaskDto> result   = new(dtos, totalCount, query.Page, query.PageSize);

        return Result.Success(result);
    }
}
