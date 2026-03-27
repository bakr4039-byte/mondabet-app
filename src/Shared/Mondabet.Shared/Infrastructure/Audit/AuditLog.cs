namespace Mondabet.Shared.Infrastructure.Audit;

/// <summary>
/// Immutable audit record written for every mutating operation.
/// Stored in a dedicated 'audit' schema table; never updated or deleted.
/// </summary>
public sealed class AuditLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string TenantId { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;      // e.g. "Employee.Create"
    public string EntityType { get; private set; } = string.Empty;  // e.g. "Employee"
    public string EntityId { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }                   // JSON snapshot before change
    public string? NewValues { get; private set; }                   // JSON snapshot after change
    public string? IpAddress { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; } = DateTimeOffset.UtcNow;

    private AuditLog() { }

    public static AuditLog Create(
        string tenantId,
        string userId,
        string action,
        string entityType,
        string entityId,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null) =>
        new()
        {
            TenantId = tenantId,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
        };
}
