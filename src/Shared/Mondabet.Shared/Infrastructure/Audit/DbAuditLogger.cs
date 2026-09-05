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

    /// <summary>
    /// Creates the audit schema/table if missing. AuditDbContext.Database.EnsureCreatedAsync()
    /// alone does NOT work here: this connection string points at each service's own database,
    /// which already exists (it has that service's own tables) by the time this runs, and
    /// EnsureCreated is a no-op whenever the target database already exists - it never adds a
    /// table to an existing database. Raw idempotent DDL is the reliable way to add this one
    /// new table alongside a database that predates it, consistent with this codebase's
    /// EnsureCreatedAsync-only (no formal migrations) approach elsewhere.
    /// </summary>
    public static async Task EnsureAuditTableCreatedAsync(this AuditDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync("""
            IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'audit')
                EXEC('CREATE SCHEMA audit');
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AuditLogs' AND schema_id = SCHEMA_ID('audit'))
            BEGIN
                CREATE TABLE audit.AuditLogs (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    TenantId NVARCHAR(36) NOT NULL,
                    UserId NVARCHAR(36) NOT NULL,
                    Action NVARCHAR(100) NOT NULL,
                    EntityType NVARCHAR(100) NOT NULL,
                    EntityId NVARCHAR(36) NOT NULL,
                    OldValues NVARCHAR(MAX) NULL,
                    NewValues NVARCHAR(MAX) NULL,
                    IpAddress NVARCHAR(45) NULL,
                    OccurredAt DATETIMEOFFSET NOT NULL
                );
                CREATE INDEX IX_AuditLogs_TenantId_OccurredAt ON audit.AuditLogs (TenantId, OccurredAt);
            END
            """);
    }
}
