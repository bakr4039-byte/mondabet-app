using MediatR;
using Microsoft.EntityFrameworkCore;
using Mondabet.Employee.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;

namespace Mondabet.Employee.Infrastructure.Persistence;

/// <summary>
/// Per-tenant schema: tables live under tenant_{tenantId} schema.
/// Schema is set via BaseDbContext.OnModelCreating from TenantMiddleware claim.
/// </summary>
public class EmployeeDbContext : BaseDbContext
{
    public DbSet<Domain.Entities.Employee> Employees => Set<Domain.Entities.Employee>();
    public DbSet<Department> Departments => Set<Department>();

    public EmployeeDbContext(
        DbContextOptions<EmployeeDbContext> options,
        ITenantProvider tenantProvider,
        IMediator mediator)
        : base(options, tenantProvider, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Call base to apply per-tenant schema + soft-delete filters
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmployeeDbContext).Assembly);
    }
}
