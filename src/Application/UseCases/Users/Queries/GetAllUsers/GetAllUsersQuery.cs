using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.UseCases.Users.DTOs;

namespace Intap.FirstProject.Application.UseCases.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery(
    string? SearchTerm,
    bool?   IsActive,
    int     Page      = 1,
    int     PageSize  = 10,
    string  SortBy    = "FullName",
    string  SortOrder = "asc"
) : IQuery<PagedResult<UserDto>>;
