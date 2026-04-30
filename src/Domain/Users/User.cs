using Intap.FirstProject.Domain.Common;
using Intap.FirstProject.Domain.Roles;

namespace Intap.FirstProject.Domain.Users;

public sealed class User : BaseEntity
{
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;

    private User() { }

    public string    FullName       { get; private set; } = string.Empty;
    public string    Email          { get; private set; } = string.Empty;
    public string    PasswordHash   { get; private set; } = string.Empty;
    public Guid      RoleId         { get; private set; }
    public Role?     AssignedRole   { get; private set; }
    public string    Identification { get; private set; } = string.Empty;
    public string    Phone          { get; private set; } = string.Empty;
    public string    Position       { get; private set; } = string.Empty;
    public bool      IsActive       { get; private set; } = true;
    public bool      IsLocked       { get; private set; }
    public bool      IsDeleted      { get; private set; }
    public int       FailedAttempts { get; private set; }
    public DateTime? LockedUntil   { get; private set; }

    // Used by DataSeeder only
    public static User Create(string fullName, string email, string passwordHash, Guid roleId)
    {
        return new User
        {
            FullName     = fullName,
            Email        = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            RoleId       = roleId,
            IsActive     = true,
        };
    }

    // Used by CreateUserCommandHandler
    public static User CreateUser(
        string email,
        string passwordHash,
        string fullName,
        string identification,
        string phone,
        string position,
        Guid   roleId)
    {
        return new User
        {
            Email          = email.ToLowerInvariant(),
            PasswordHash   = passwordHash,
            FullName       = fullName,
            Identification = identification,
            Phone          = phone,
            Position       = position,
            RoleId         = roleId,
            IsActive       = true,
        };
    }

    public void AssignRole(Guid roleId)
    {
        RoleId = roleId;
        SetUpdatedAt();
    }

    public void Update(string email, string fullName, string identification, string phone, string position)
    {
        Email          = email.ToLowerInvariant();
        FullName       = fullName;
        Identification = identification;
        Phone          = phone;
        Position       = position;
        SetUpdatedAt();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void Delete()
    {
        IsDeleted = true;
        SetUpdatedAt();
    }

    public void ResetPassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        IsLocked = false;
        FailedAttempts = 0;
        LockedUntil = null;
        SetUpdatedAt();
    }

    public void RegisterFailedAttempt()
    {
        FailedAttempts++;
        if (FailedAttempts >= MaxFailedAttempts)
        {
            IsLocked = true;
            LockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
        }
        SetUpdatedAt();
    }

    public void ResetFailedAttempts()
    {
        FailedAttempts = 0;
        IsLocked = false;
        LockedUntil = null;
        SetUpdatedAt();
    }

    public bool CheckLockExpiry()
    {
        if (IsLocked && LockedUntil.HasValue && DateTime.UtcNow > LockedUntil.Value)
        {
            ResetFailedAttempts();
            return false;
        }
        return IsLocked;
    }
}
