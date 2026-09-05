using MediatR;
using Mondabet.Leave.Domain.Events;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure.Audit;

namespace Mondabet.Leave.Application.EventHandlers;

/// <summary>
/// Wires LeaveApprovedEvent/LeaveRejectedEvent - already raised by LeaveRequest.Approve/Reject,
/// but until now never handled by anything - to the audit log infrastructure. Same pattern as
/// Identity's UserLoggedInAuditHandler.
/// </summary>
public class LeaveApprovedAuditHandler : INotificationHandler<LeaveApprovedEvent>
{
    private readonly IAuditLogger _auditLogger;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserProvider _currentUser;

    public LeaveApprovedAuditHandler(
        IAuditLogger auditLogger, ITenantProvider tenantProvider, ICurrentUserProvider currentUser)
    {
        _auditLogger = auditLogger;
        _tenantProvider = tenantProvider;
        _currentUser = currentUser;
    }

    public Task Handle(LeaveApprovedEvent notification, CancellationToken cancellationToken)
        => _auditLogger.LogAsync(AuditLog.Create(
            tenantId: _tenantProvider.TenantId?.ToString() ?? string.Empty,
            userId: _currentUser.UserId?.ToString() ?? string.Empty,
            action: "Leave.Approve",
            entityType: "LeaveRequest",
            entityId: notification.LeaveRequestId.ToString(),
            ipAddress: _currentUser.IpAddress), cancellationToken);
}

public class LeaveRejectedAuditHandler : INotificationHandler<LeaveRejectedEvent>
{
    private readonly IAuditLogger _auditLogger;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserProvider _currentUser;

    public LeaveRejectedAuditHandler(
        IAuditLogger auditLogger, ITenantProvider tenantProvider, ICurrentUserProvider currentUser)
    {
        _auditLogger = auditLogger;
        _tenantProvider = tenantProvider;
        _currentUser = currentUser;
    }

    public Task Handle(LeaveRejectedEvent notification, CancellationToken cancellationToken)
        => _auditLogger.LogAsync(AuditLog.Create(
            tenantId: _tenantProvider.TenantId?.ToString() ?? string.Empty,
            userId: _currentUser.UserId?.ToString() ?? string.Empty,
            action: "Leave.Reject",
            entityType: "LeaveRequest",
            entityId: notification.LeaveRequestId.ToString(),
            ipAddress: _currentUser.IpAddress), cancellationToken);
}
