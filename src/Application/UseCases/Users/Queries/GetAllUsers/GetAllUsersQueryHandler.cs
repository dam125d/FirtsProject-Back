using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Users.DTOs;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Users.Queries.GetAllUsers;

internal sealed class GetAllUsersQueryHandler(
    IUserReadRepository userReadRepository,
    IMapper             mapper
) : IQueryHandler<GetAllUsersQuery, PagedResult<UserDto>>
{
    public async Task<Result<PagedResult<UserDto>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        (List<User> items, int totalCount) = await userReadRepository.GetPagedAsync(
            query.SearchTerm,
            query.IsActive,
            query.Page,
            query.PageSize,
            query.SortBy,
            query.SortOrder,
            cancellationToken);

        List<UserDto> dtos = mapper.Map<List<UserDto>>(items);
        PagedResult<UserDto> result = new(dtos, totalCount, query.Page, query.PageSize);

        return Result.Success(result);
    }
}
