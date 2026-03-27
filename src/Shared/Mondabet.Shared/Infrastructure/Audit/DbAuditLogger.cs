using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mondabet.Shared.Infrastructure.Audit;

/// <summary>
/// Persists audit logs to a dedicated AuditLogs table in the shared 'audit' schema.
/// Each service resolves this via the DI container; the table is created by the
/// shared migration in the platform database (not per-tenant).
/// </summary>
public sealed class DbAuditLogger : IAuditLogger
{
    private readonly AuditDbContext _db;

    public DbAuditLogger(AuditDbContext db) => _db = db;

    public async Task LogAsync(AuditLog entry, CancellationToken ct = default)
    {
        _db.AuditLogs.Add(entry);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}

/// <summary>Minimal DbContext for the audit table — shared across all services.</summary>
public sealed class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("audit");
        modelBuilder.Entity<AuditLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TenantId).HasMaxLength(36).IsRequired();
            e.Property(x => x.UserId).HasMaxLength(36).IsRequired();
            e.Property(x => x.Action).HasMaxLength(100).IsRequired();
            e.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            e.Property(x => x.EntityId).HasMaxLength(36).IsRequired();
            e.Property(x => x.OldValues).HasColumnType("nvarchar(max)");
            e.Property(x => x.NewValues).HasColumnType("nvarchar(max)");
            e.Property(x => x.IpAddress).HasMaxLength(45);
            e.HasIndex(x => new { x.TenantId, x.OccurredAt });
        });
    }
}

public static class AuditServiceExtensions
{
    public static IServiceCollection AddAuditLogging(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AuditDbContext>(o => o.UseSqlServer(connectionString));
        services.AddScoped<IAuditLogger, DbAuditLogger>();
        return services;
    }
}
