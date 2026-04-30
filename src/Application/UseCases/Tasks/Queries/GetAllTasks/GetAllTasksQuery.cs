using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.UseCases.Tasks.DTOs;

namespace Intap.FirstProject.Application.UseCases.Tasks.Queries.GetAllTasks;

public sealed record GetAllTasksQuery(
    Guid?  ProjectId = null,
    Guid?  UserId    = null,
    string? Status   = null,
    string? Date     = null,
    int    Page      = 1,
    int    PageSize  = 50
) : IQuery<PagedResult<TaskDto>>;
