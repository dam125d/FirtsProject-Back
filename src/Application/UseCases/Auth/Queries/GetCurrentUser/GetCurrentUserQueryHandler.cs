using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Auth.DTOs;
using Intap.FirstProject.Application.UseCases.Auth.Errors;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Auth.Queries.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler(
    ICurrentUser currentUser,
    IUserReadRepository userRepository) : IQueryHandler<GetCurrentUserQuery, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);

        User? user = await userRepository.GetByIdWithRoleAsync(currentUser.UserId.Value, cancellationToken);

        if (user is null)
            return Result.Failure<LoginResponse>(UserErrors.NotFound);

        if (!user.IsActive)
            return Result.Failure<LoginResponse>(UserErrors.AccountInactive);

        if (user.AssignedRole is null || !user.AssignedRole.IsActive)
            return Result.Failure<LoginResponse>(UserErrors.AccountInactive);

        LoginResponse response = new(
            user.Id,
            user.FullName,
            user.Email,
            user.AssignedRole.Name,
            user.AssignedRole.GetPermissionCodes()
        );

        return Result.Success(response);
    }
}
