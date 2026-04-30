using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Auth.DTOs;

namespace Intap.FirstProject.Application.UseCases.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
