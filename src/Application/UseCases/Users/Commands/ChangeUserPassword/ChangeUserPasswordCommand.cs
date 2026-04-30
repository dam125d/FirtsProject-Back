using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Users.Commands.ChangeUserPassword;

public sealed record ChangeUserPasswordCommand(
    Guid   UserId,
    string NewPassword,
    string ConfirmPassword
) : ICommand;
