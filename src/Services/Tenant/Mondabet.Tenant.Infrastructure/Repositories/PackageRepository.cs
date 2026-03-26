using Microsoft.EntityFrameworkCore;
using Mondabet.Tenant.Application.Interfaces;
using Mondabet.Tenant.Domain.Entities;
using Mondabet.Tenant.Infrastructure.Persistence;

namespace Mondabet.Tenant.Infrastructure.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly TenantDbContext _ctx;
    public PackageRepository(TenantDbContext ctx) => _ctx = ctx;

    public async Task<Package?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Packages.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Package>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Packages.ToListAsync(ct);

    public async Task AddAsync(Package entity, CancellationToken ct = default)
        => await _ctx.Packages.AddAsync(entity, ct);

    public void Update(Package entity) => _ctx.Packages.Update(entity);

    public void Delete(Package entity)
    {
        entity.IsDeleted = true;
        _ctx.Packages.Update(entity);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Packages.AnyAsync(p => p.Id == id, ct);
}
