using Intap.FirstProject.Application.Abstractions.Results;

namespace Intap.FirstProject.Application.UseCases.Auth.Errors;

public static class UserErrors
{
    public static readonly ErrorResult InvalidCredentials =
        new("Auth.InvalidCredentials", "Email or password is incorrect.", ErrorTypeResult.Unauthorized);

    public static readonly ErrorResult AccountLocked =
        new("Auth.AccountLocked", "Account is temporarily locked due to multiple failed attempts.", ErrorTypeResult.Unauthorized);

    public static readonly ErrorResult AccountInactive =
        new("Auth.AccountInactive", "Account is disabled.", ErrorTypeResult.Unauthorized);

    public static readonly ErrorResult NotFound =
        new("User.NotFound", "User was not found.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult EmailAlreadyExists =
        new("User.EmailAlreadyExists", "A user with this email already exists.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult CannotDeleteSelf =
        new("User.CannotDeleteSelf", "You cannot delete your own account.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult CannotDeactivateSelf =
        new("User.CannotDeactivateSelf", "You cannot deactivate your own account.", ErrorTypeResult.Conflict);
}
