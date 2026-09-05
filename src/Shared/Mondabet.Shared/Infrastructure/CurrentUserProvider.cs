using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Mondabet.Shared.Application;

namespace Mondabet.Shared.Infrastructure;

/// <summary>
/// Reads the current user's id from the "sub" JWT claim (the same claim JwtTokenService signs
/// into every access token) via the current HttpContext. Requires AddHttpContextAccessor() to
/// be registered by the consuming service.
/// </summary>
public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? IpAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
}
