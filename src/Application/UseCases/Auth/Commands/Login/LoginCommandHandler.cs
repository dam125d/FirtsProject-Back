using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Auth.DTOs;
using Intap.FirstProject.Application.UseCases.Auth.Errors;
using Intap.FirstProject.Domain.Users;
using Microsoft.AspNetCore.Http;

namespace Intap.FirstProject.Application.UseCases.Auth.Commands.Login;

internal sealed class LoginCommandHandler(
    IUserReadRepository userRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor) : ICommandHandler<LoginCommand, LoginResponse>
{
    private const string CookieName = "intap-auth";

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailWithRoleAsync(command.Email.ToLowerInvariant(), cancellationToken);

        if (user is null)
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);

        if (!user.IsActive)
            return Result.Failure<LoginResponse>(UserErrors.AccountInactive);

        if (user.AssignedRole is null || !user.AssignedRole.IsActive)
            return Result.Failure<LoginResponse>(UserErrors.AccountInactive);

        bool isStillLocked = user.CheckLockExpiry();
        if (isStillLocked)
            return Result.Failure<LoginResponse>(UserErrors.AccountLocked);

        bool passwordValid = BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash);
        if (!passwordValid)
        {
            user.RegisterFailedAttempt();
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
        }

        user.ResetFailedAttempts();
        await unitOfWork.CompleteAsync(cancellationToken);

        string token = tokenService.GenerateToken(user);

        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            httpContext.Response.Cookies.Append(CookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(8),
                Path = "/",
            });
        }

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
