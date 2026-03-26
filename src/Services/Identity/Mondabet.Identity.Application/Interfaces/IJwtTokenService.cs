using Mondabet.Identity.Domain.Entities;

namespace Mondabet.Identity.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    Guid? GetUserIdFromToken(string token);
}
