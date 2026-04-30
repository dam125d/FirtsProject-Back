using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Roles.DTOs;
using Intap.FirstProject.Application.UseCases.Roles.Errors;
using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Application.UseCases.Roles.Queries.GetRoleById;

internal sealed class GetRoleByIdQueryHandler(
    IRoleReadRepository roleReadRepository
) : IQueryHandler<GetRoleByIdQuery, RoleDetailDto>
{
    public async Task<Result<RoleDetailDto>> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
    {
        Role? role = await roleReadRepository.GetByIdWithPermissionsAsync(query.Id, cancellationToken);
        if (role is null)
            return Result.Failure<RoleDetailDto>(RoleErrors.NotFound);

        RoleDetailDto dto = new(
            role.Id,
            role.Name,
            role.Description,
            role.IsActive,
            role.IsSystem,
            role.GetPermissionCodes());

        return Result.Success(dto);
    }
}
