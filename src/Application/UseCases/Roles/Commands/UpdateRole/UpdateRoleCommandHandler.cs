using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Roles.Errors;
using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Application.UseCases.Roles.Commands.UpdateRole;

internal sealed class UpdateRoleCommandHandler(
    IRoleWriteRepository roleWriteRepository,
    IRoleReadRepository  roleReadRepository,
    IUnitOfWork          unitOfWork
) : ICommandHandler<UpdateRoleCommand>
{
    public async Task<Result> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        Role? role = await roleWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (role is null)
            return Result.Failure(RoleErrors.NotFound);

        if (role.IsSystem && !string.Equals(role.Name, command.Name, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(RoleErrors.SystemRoleCannotBeModified);

        bool nameExists = await roleReadRepository.ExistsByNameAsync(command.Name, cancellationToken);
        if (nameExists && !string.Equals(role.Name, command.Name, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(RoleErrors.AlreadyExists);

        role.Update(command.Name, command.Description);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
