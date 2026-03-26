using MediatR;
using Microsoft.EntityFrameworkCore;
using Mondabet.Identity.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;

namespace Mondabet.Identity.Infrastructure.Persistence;

public class IdentityDbContext : BaseDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<BiometricDevice> BiometricDevices => Set<BiometricDevice>();

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        ITenantProvider tenantProvider,
        IMediator mediator)
        : base(options, tenantProvider, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity tables live in dbo (global schema) – skip per-tenant schema
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        // Do NOT call base which sets per-tenant schema
        // Apply soft-delete filter only
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<BiometricDevice>().HasQueryFilter(b => !b.IsDeleted);
    }
}
