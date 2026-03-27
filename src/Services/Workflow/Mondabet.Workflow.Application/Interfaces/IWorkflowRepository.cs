using Mondabet.Shared.Application;
using Mondabet.Workflow.Domain.Entities;

namespace Mondabet.Workflow.Application.Interfaces;

public interface IWorkflowDefinitionRepository : IRepository<WorkflowDefinition>
{
}

public interface IWorkflowInstanceRepository : IRepository<WorkflowInstance>
{
    Task<IReadOnlyList<WorkflowInstance>> GetByDefinitionAsync(
        Guid definitionId, CancellationToken ct = default);
    Task<WorkflowInstance?> GetActiveByEntityAsync(
        Guid entityId, CancellationToken ct = default);
}
