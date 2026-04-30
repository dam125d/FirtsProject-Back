using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Roles.Errors;
using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Application.UseCases.Roles.Commands.DeleteRole;

internal sealed class DeleteRoleCommandHandler(
    IRoleWriteRepository roleWriteRepository,
    IRoleReadRepository  roleReadRepository,
    IUnitOfWork          unitOfWork
) : ICommandHandler<DeleteRoleCommand>
{
    public async Task<Result> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        Role? role = await roleWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (role is null)
            return Result.Failure(RoleErrors.NotFound);

        if (role.IsSystem)
            return Result.Failure(RoleErrors.SystemRoleCannotBeModified);

        bool hasUsers = await roleReadRepository.HasUsersAssignedAsync(command.Id, cancellationToken);
        if (hasUsers)
            return Result.Failure(RoleErrors.RoleInUse);

        roleWriteRepository.Remove(role);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
