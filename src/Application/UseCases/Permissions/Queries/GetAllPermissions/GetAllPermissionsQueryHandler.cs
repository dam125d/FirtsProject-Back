using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Permissions.DTOs;
using Intap.FirstProject.Domain.Permissions;

namespace Intap.FirstProject.Application.UseCases.Permissions.Queries.GetAllPermissions;

internal sealed class GetAllPermissionsQueryHandler(
    IPermissionReadRepository permissionReadRepository
) : IQueryHandler<GetAllPermissionsQuery, List<PermissionDto>>
{
    public async Task<Result<List<PermissionDto>>> Handle(GetAllPermissionsQuery query, CancellationToken cancellationToken)
    {
        List<Permission> permissions = await permissionReadRepository.GetAllAsync(cancellationToken);

        List<PermissionDto> dtos = permissions
            .Select(p => new PermissionDto(p.Code, p.Name, p.Module))
            .ToList();

        return Result.Success(dtos);
    }
}
