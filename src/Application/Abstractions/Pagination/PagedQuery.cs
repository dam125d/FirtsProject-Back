namespace Intap.FirstProject.Application.Abstractions.Pagination;

public abstract record PagedQuery(int Page = 1, int PageSize = 10, string? Search = null);
