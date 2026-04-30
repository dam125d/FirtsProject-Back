using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Auth.Errors;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.ChangeUserPassword;

internal sealed class ChangeUserPasswordCommandHandler(
    IUserWriteRepository userWriteRepository,
    IUnitOfWork          unitOfWork
) : ICommandHandler<ChangeUserPasswordCommand>
{
    public async Task<Result> Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        User? user = await userWriteRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword, workFactor: 12);
        user.ChangePassword(newPasswordHash);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
