using MediatR;
using Microsoft.EntityFrameworkCore;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;
using Mondabet.Tenant.Domain.Entities;

namespace Mondabet.Tenant.Infrastructure.Persistence;

public class TenantDbContext : BaseDbContext
{
    public DbSet<Domain.Entities.Tenant> Tenants => Set<Domain.Entities.Tenant>();
    public DbSet<Package> Packages => Set<Package>();

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        ITenantProvider tenantProvider,
        IMediator mediator)
        : base(options, tenantProvider, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tenant and Package live in dbo – skip per-tenant schema
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
        modelBuilder.Entity<Domain.Entities.Tenant>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<Package>().HasQueryFilter(p => !p.IsDeleted);
    }
}
