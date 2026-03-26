using Mondabet.Shared.Application;
using Mondabet.Tenant.Domain.Entities;

namespace Mondabet.Tenant.Application.Interfaces;

public interface IPackageRepository : IRepository<Package>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
