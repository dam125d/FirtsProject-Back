using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Auth.Errors;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.DeleteUser;

internal sealed class DeleteUserCommandHandler(
    IUserWriteRepository userWriteRepository,
    ICurrentUser         currentUser,
    IUnitOfWork          unitOfWork
) : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await userWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (currentUser.UserId == command.Id)
            return Result.Failure(UserErrors.CannotDeleteSelf);

        user.Delete();
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
