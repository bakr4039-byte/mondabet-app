using Microsoft.AspNetCore.Http;

namespace Mondabet.Shared.Infrastructure;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, TenantProvider tenantProvider)
    {
        var tenantClaim = context.User.FindFirst("tid")?.Value;
        var headerTenant = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();

        var tenantIdStr = tenantClaim ?? headerTenant;

        if (Guid.TryParse(tenantIdStr, out var tenantId))
        {
            tenantProvider.SetTenant(tenantId, null);
        }

        await _next(context);
    }
}
