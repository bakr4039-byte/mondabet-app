using MediatR;
using Mondabet.Identity.Domain.Events;
using Mondabet.Shared.Infrastructure.Audit;

namespace Mondabet.Identity.Application.EventHandlers;

/// <summary>
/// Wires UserLoggedInEvent - raised on every successful MFA-verified login, but until now
/// never handled by anything - to the audit log infrastructure that already existed in
/// Mondabet.Shared but was never actually connected to any service. One immutable row per
/// successful login: who, when, from which tenant.
/// </summary>
public class UserLoggedInAuditHandler : INotificationHandler<UserLoggedInEvent>
{
    private readonly IAuditLogger _auditLogger;

    public UserLoggedInAuditHandler(IAuditLogger auditLogger) => _auditLogger = auditLogger;

    public Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
    {
        var entry = AuditLog.Create(
            tenantId: notification.TenantId?.ToString() ?? string.Empty,
            userId: notification.UserId.ToString(),
            action: "User.Login",
            entityType: "User",
            entityId: notification.UserId.ToString());

        return _auditLogger.LogAsync(entry, cancellationToken);
    }
}
