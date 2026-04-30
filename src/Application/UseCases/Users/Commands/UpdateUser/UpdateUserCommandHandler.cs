using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Auth.Errors;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler(
    IUserReadRepository  userReadRepository,
    IUserWriteRepository userWriteRepository,
    IUnitOfWork          unitOfWork
) : ICommandHandler<UpdateUserCommand>
{
    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await userWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        bool emailExists = await userReadRepository.ExistsByEmailAsync(command.Email, command.Id, cancellationToken);
        if (emailExists)
            return Result.Failure(UserErrors.EmailAlreadyExists);

        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            string newHash = BCrypt.Net.BCrypt.HashPassword(command.Password, workFactor: 12);
            user.ChangePassword(newHash);
        }

        user.Update(command.Email, command.FullName, command.Identification, command.Phone, command.Position);
        user.AssignRole(command.RoleId);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
