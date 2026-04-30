using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Auth.Errors;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUserReadRepository  userReadRepository,
    IUserWriteRepository userWriteRepository,
    IUnitOfWork          unitOfWork
) : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        bool emailExists = await userReadRepository.ExistsByEmailAsync(command.Email, null, cancellationToken);
        if (emailExists)
            return Result.Failure<Guid>(UserErrors.EmailAlreadyExists);

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password, workFactor: 12);

        User user = User.CreateUser(
            command.Email,
            passwordHash,
            command.FullName,
            command.Identification,
            command.Phone,
            command.Position,
            command.RoleId);

        userWriteRepository.Add(user);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
