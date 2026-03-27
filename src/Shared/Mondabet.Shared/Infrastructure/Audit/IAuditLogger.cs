namespace Mondabet.Shared.Infrastructure.Audit;

public interface IAuditLogger
{
    Task LogAsync(AuditLog entry, CancellationToken ct = default);
}
