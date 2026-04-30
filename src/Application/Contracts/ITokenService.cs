using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.Contracts;

public interface ITokenService
{
    string GenerateToken(User user);
}
