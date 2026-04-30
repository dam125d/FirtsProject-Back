using System;

namespace Intap.FirstProject.Application.Contracts
{
    public interface ICurrentUser
    {
        Guid? UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}
