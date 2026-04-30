using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Roles.Errors;
using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Application.UseCases.Roles.Commands.CreateRole;

internal sealed class CreateRoleCommandHandler(
    IRoleReadRepository  roleReadRepository,
    IRoleWriteRepository roleWriteRepository,
    IUnitOfWork          unitOfWork
) : ICommandHandler<CreateRoleCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        bool nameExists = await roleReadRepository.ExistsByNameAsync(command.Name, cancellationToken);
        if (nameExists)
            return Result.Failure<Guid>(RoleErrors.AlreadyExists);

        Role role = Role.Create(command.Name, command.Description);

        roleWriteRepository.Add(role);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(role.Id);
    }
}
