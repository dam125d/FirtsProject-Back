using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Microsoft.AspNetCore.Http;

namespace Intap.FirstProject.Application.UseCases.Auth.Commands.Logout;

internal sealed class LogoutCommandHandler(
    IHttpContextAccessor httpContextAccessor) : ICommandHandler<LogoutCommand>
{
    private const string CookieName = "intap-auth";

    public Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            httpContext.Response.Cookies.Append(CookieName, string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UnixEpoch,
                MaxAge = TimeSpan.Zero,
                Path = "/",
            });
        }

        return Task.FromResult(Result.Success());
    }
}
