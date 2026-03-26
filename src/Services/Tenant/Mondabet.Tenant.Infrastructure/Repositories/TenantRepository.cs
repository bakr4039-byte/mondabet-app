using Microsoft.EntityFrameworkCore;
using Mondabet.Tenant.Application.Interfaces;
using Mondabet.Tenant.Infrastructure.Persistence;

namespace Mondabet.Tenant.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly TenantDbContext _ctx;
    public TenantRepository(TenantDbContext ctx) => _ctx = ctx;

    public async Task<Domain.Entities.Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Domain.Entities.Tenant>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Tenants.ToListAsync(ct);

    public async Task AddAsync(Domain.Entities.Tenant entity, CancellationToken ct = default)
        => await _ctx.Tenants.AddAsync(entity, ct);

    public void Update(Domain.Entities.Tenant entity) => _ctx.Tenants.Update(entity);

    public void Delete(Domain.Entities.Tenant entity)
    {
        entity.IsDeleted = true;
        _ctx.Tenants.Update(entity);
    }

    public async Task<Domain.Entities.Tenant?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _ctx.Tenants.FirstOrDefaultAsync(t => t.Code == code.ToLowerInvariant(), ct);

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => await _ctx.Tenants.AnyAsync(t => t.Code == code.ToLowerInvariant(), ct);
}
