using Mondabet.Shared.Domain;

namespace Mondabet.Workflow.Domain.Events;

public record WorkflowCompletedEvent(Guid InstanceId, Guid EntityId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
