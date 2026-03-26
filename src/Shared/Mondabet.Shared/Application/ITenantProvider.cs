namespace Mondabet.Shared.Application;

public interface ITenantProvider
{
    Guid? TenantId { get; }
    string? TenantCode { get; }
}
