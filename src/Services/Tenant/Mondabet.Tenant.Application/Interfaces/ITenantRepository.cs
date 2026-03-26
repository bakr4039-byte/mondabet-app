using Mondabet.Shared.Application;
using TenantEntity = Mondabet.Tenant.Domain.Entities.Tenant;

namespace Mondabet.Tenant.Application.Interfaces;

public interface ITenantRepository : IRepository<TenantEntity>
{
    Task<TenantEntity?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
}
