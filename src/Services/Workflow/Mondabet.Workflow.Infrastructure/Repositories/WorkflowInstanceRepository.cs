using Microsoft.EntityFrameworkCore;
using Mondabet.Workflow.Application.Interfaces;
using Mondabet.Workflow.Domain.Entities;
using Mondabet.Workflow.Infrastructure.Persistence;

namespace Mondabet.Workflow.Infrastructure.Repositories;

public class WorkflowInstanceRepository : IWorkflowInstanceRepository
{
    private readonly WorkflowDbContext _ctx;
    public WorkflowInstanceRepository(WorkflowDbContext ctx) => _ctx = ctx;

    public async Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.WorkflowInstances.FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<IReadOnlyList<WorkflowInstance>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.WorkflowInstances.ToListAsync(ct);

    public async Task AddAsync(WorkflowInstance entity, CancellationToken ct = default)
        => await _ctx.WorkflowInstances.AddAsync(entity, ct);

    public void Update(WorkflowInstance entity) => _ctx.WorkflowInstances.Update(entity);

    public void Delete(WorkflowInstance entity)
    {
        entity.IsDeleted = true;
        _ctx.WorkflowInstances.Update(entity);
    }

    public async Task<IReadOnlyList<WorkflowInstance>> GetByDefinitionAsync(
        Guid definitionId, CancellationToken ct = default)
        => await _ctx.WorkflowInstances
            .Where(w => w.DefinitionId == definitionId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(ct);

    public async Task<WorkflowInstance?> GetActiveByEntityAsync(
        Guid entityId, CancellationToken ct = default)
        => await _ctx.WorkflowInstances.FirstOrDefaultAsync(
            w => w.EntityId == entityId
              && w.Status == WorkflowInstanceStatus.InProgress,
            ct);
}
