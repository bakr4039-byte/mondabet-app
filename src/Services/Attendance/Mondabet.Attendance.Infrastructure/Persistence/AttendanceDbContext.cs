using MediatR;
using Microsoft.EntityFrameworkCore;
using Mondabet.Attendance.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;

namespace Mondabet.Attendance.Infrastructure.Persistence;

/// <summary>Per-tenant schema — tables live under tenant_{tenantId} schema.</summary>
public class AttendanceDbContext : BaseDbContext
{
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    public AttendanceDbContext(
        DbContextOptions<AttendanceDbContext> options,
        ITenantProvider tenantProvider,
        IMediator mediator)
        : base(options, tenantProvider, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendanceDbContext).Assembly);
    }
}
