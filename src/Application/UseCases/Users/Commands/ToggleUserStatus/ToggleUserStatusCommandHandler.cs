using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Auth.Errors;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.ToggleUserStatus;

internal sealed class ToggleUserStatusCommandHandler(
    IUserWriteRepository userWriteRepository,
    ICurrentUser         currentUser,
    IUnitOfWork          unitOfWork
) : ICommandHandler<ToggleUserStatusCommand>
{
    public async Task<Result> Handle(ToggleUserStatusCommand command, CancellationToken cancellationToken)
    {
        User? user = await userWriteRepository.GetByIdAsync(command.Id, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (user.IsActive && currentUser.UserId == command.Id)
            return Result.Failure(UserErrors.CannotDeactivateSelf);

        if (user.IsActive)
            user.Deactivate();
        else
            user.Activate();

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
