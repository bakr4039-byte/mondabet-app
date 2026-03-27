using Mondabet.Workflow.Domain.Events;
using Mondabet.Shared.Domain;

namespace Mondabet.Workflow.Domain.Entities;

public enum WorkflowInstanceStatus : byte { InProgress = 1, Completed = 2, Rejected = 3 }

public class WorkflowInstance : BaseEntity
{
    public Guid DefinitionId { get; private set; }
    public Guid EntityId { get; private set; }   // LeaveRequestId
    public int CurrentStep { get; private set; }
    public WorkflowInstanceStatus Status { get; private set; }
    public int TotalSteps { get; private set; }

    private WorkflowInstance() { }

    public static WorkflowInstance Create(
        Guid definitionId, Guid entityId, int totalSteps) => new()
    {
        Id = Guid.NewGuid(),
        DefinitionId = definitionId,
        EntityId = entityId,
        CurrentStep = 1,
        TotalSteps = totalSteps,
        Status = WorkflowInstanceStatus.InProgress,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    /// <summary>Move to next step. If last step, mark Completed.</summary>
    public void Advance()
    {
        if (CurrentStep >= TotalSteps)
        {
            Status = WorkflowInstanceStatus.Completed;
            AddDomainEvent(new WorkflowCompletedEvent(Id, EntityId));
        }
        else
        {
            CurrentStep++;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = WorkflowInstanceStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new WorkflowRejectedEvent(Id, EntityId));
    }
}
