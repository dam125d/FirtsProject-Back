using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Auth.Commands.Login;
using Intap.FirstProject.Application.UseCases.Auth.Commands.Logout;
using Intap.FirstProject.Application.UseCases.Auth.DTOs;
using Intap.FirstProject.Application.UseCases.Auth.Queries.GetCurrentUser;
using Intap.FirstProject.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Controllers;

public sealed class AuthController(
    ICommandHandler<LoginCommand, LoginResponse> loginHandler,
    ICommandHandler<LogoutCommand> logoutHandler,
    IQueryHandler<GetCurrentUserQuery, LoginResponse> getMeHandler) : ApiController
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct) =>
        HandleResult(await loginHandler.Handle(command, ct));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct) =>
        HandleResult(await logoutHandler.Handle(new LogoutCommand(), ct));

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct) =>
        HandleResult(await getMeHandler.Handle(new GetCurrentUserQuery(), ct));
}
