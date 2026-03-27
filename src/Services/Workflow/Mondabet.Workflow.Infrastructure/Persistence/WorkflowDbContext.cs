using MediatR;
using Microsoft.EntityFrameworkCore;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;
using Mondabet.Workflow.Domain.Entities;

namespace Mondabet.Workflow.Infrastructure.Persistence;

/// <summary>
/// Workflow definitions and instances live in per-tenant schema.
/// Each tenant configures their own approval workflows.
/// </summary>
public class WorkflowDbContext : BaseDbContext
{
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();

    public WorkflowDbContext(
        DbContextOptions<WorkflowDbContext> options,
        ITenantProvider tenantProvider,
        IMediator mediator)
        : base(options, tenantProvider, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowDbContext).Assembly);
    }
}
