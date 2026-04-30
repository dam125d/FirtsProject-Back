using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Roles.DTOs;
using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Application.UseCases.Roles.Queries.GetAllRoles;

internal sealed class GetAllRolesQueryHandler(
    IRoleReadRepository roleReadRepository
) : IQueryHandler<GetAllRolesQuery, List<RoleDto>>
{
    public async Task<Result<List<RoleDto>>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
    {
        List<Role> roles = await roleReadRepository.GetAllAsync(cancellationToken);

        List<RoleDto> dtos = roles
            .Select(r => new RoleDto(
                r.Id,
                r.Name,
                r.Description,
                r.IsActive,
                r.IsSystem,
                r.Permissions.Count))
            .ToList();

        return Result.Success(dtos);
    }
}
