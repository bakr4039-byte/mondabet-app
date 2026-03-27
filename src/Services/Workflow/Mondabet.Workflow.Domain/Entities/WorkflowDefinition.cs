using Mondabet.Shared.Domain;

namespace Mondabet.Workflow.Domain.Entities;

/// <summary>AppliesTo values match LeaveType: 1=Vacation 2=Permission 3=Both.</summary>
public enum WorkflowAppliesTo : byte { Vacation = 1, Permission = 2, Both = 3 }

public class WorkflowDefinition : BaseEntity
{
    public string Name { get; private set; } = default!;
    public WorkflowAppliesTo AppliesTo { get; private set; }
    /// <summary>JSON: [{order, approverRoleId, escalationHours}]</summary>
    public string StepsJson { get; private set; } = "[]";

    private WorkflowDefinition() { }

    public static WorkflowDefinition Create(
        string name, WorkflowAppliesTo appliesTo, string stepsJson) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        AppliesTo = appliesTo,
        StepsJson = stepsJson,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public void Update(string name, WorkflowAppliesTo appliesTo, string stepsJson)
    {
        Name = name;
        AppliesTo = appliesTo;
        StepsJson = stepsJson;
        UpdatedAt = DateTime.UtcNow;
    }
}
