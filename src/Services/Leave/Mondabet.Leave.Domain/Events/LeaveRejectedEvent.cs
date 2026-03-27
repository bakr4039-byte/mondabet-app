using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Domain.Events;

public record LeaveRejectedEvent(Guid LeaveRequestId, Guid EmployeeId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
