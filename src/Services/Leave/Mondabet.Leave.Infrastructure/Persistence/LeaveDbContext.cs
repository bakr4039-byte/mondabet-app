using MediatR;
using Microsoft.EntityFrameworkCore;
using Mondabet.Leave.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;

namespace Mondabet.Leave.Infrastructure.Persistence;

public class LeaveDbContext : BaseDbContext
{
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveAttachment> LeaveAttachments => Set<LeaveAttachment>();

    public LeaveDbContext(
        DbContextOptions<LeaveDbContext> options,
        ITenantProvider tenantProvider,
        IMediator mediator)
        : base(options, tenantProvider, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeaveDbContext).Assembly);
    }
}
