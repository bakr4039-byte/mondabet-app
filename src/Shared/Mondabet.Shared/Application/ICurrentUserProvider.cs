namespace Mondabet.Shared.Application;

/// <summary>
/// Resolves the currently authenticated user (and their request IP) from the current HTTP
/// request, for use in audit logging and similar cross-cutting concerns. Mirrors
/// <see cref="ITenantProvider"/>'s split between an Application-facing interface and its
/// Infrastructure-side HttpContext-backed implementation.
/// </summary>
public interface ICurrentUserProvider
{
    Guid? UserId { get; }
    string? IpAddress { get; }
}
