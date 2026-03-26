using Mondabet.Shared.Application;

namespace Mondabet.Shared.Infrastructure;

public class TenantProvider : ITenantProvider
{
    public Guid? TenantId { get; private set; }
    public string? TenantCode { get; private set; }

    public void SetTenant(Guid tenantId, string? tenantCode)
    {
        TenantId = tenantId;
        TenantCode = tenantCode;
    }
}
