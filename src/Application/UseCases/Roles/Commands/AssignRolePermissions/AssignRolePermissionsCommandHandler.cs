using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Roles.Errors;
using Intap.FirstProject.Domain.Permissions;
using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Application.UseCases.Roles.Commands.AssignRolePermissions;

internal sealed class AssignRolePermissionsCommandHandler(
    IRoleWriteRepository       roleWriteRepository,
    IPermissionReadRepository  permissionReadRepository,
    IUnitOfWork                unitOfWork
) : ICommandHandler<AssignRolePermissionsCommand>
{
    public async Task<Result> Handle(AssignRolePermissionsCommand command, CancellationToken cancellationToken)
    {
        Role? role = await roleWriteRepository.GetByIdWithPermissionsAsync(command.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(RoleErrors.NotFound);

        List<Permission> existingPermissions = await permissionReadRepository.GetByCodesAsync(command.PermissionCodes, cancellationToken);

        if (existingPermissions.Count != command.PermissionCodes.Count)
        {
            IEnumerable<string> unknownCodes = command.PermissionCodes
                .Except(existingPermissions.Select(p => p.Code));

            string unknownList = string.Join(", ", unknownCodes);

            return Result.Failure(new ErrorResult(
                "Permission.NotFound",
                $"The following permission codes do not exist: {unknownList}.",
                ErrorTypeResult.Validation));
        }

        role.AssignPermissions(command.PermissionCodes);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
