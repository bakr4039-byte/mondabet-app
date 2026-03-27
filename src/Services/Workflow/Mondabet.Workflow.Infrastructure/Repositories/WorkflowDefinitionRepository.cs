using Microsoft.EntityFrameworkCore;
using Mondabet.Workflow.Application.Interfaces;
using Mondabet.Workflow.Domain.Entities;
using Mondabet.Workflow.Infrastructure.Persistence;

namespace Mondabet.Workflow.Infrastructure.Repositories;

public class WorkflowDefinitionRepository : IWorkflowDefinitionRepository
{
    private readonly WorkflowDbContext _ctx;
    public WorkflowDefinitionRepository(WorkflowDbContext ctx) => _ctx = ctx;

    public async Task<WorkflowDefinition?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.WorkflowDefinitions.FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<IReadOnlyList<WorkflowDefinition>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.WorkflowDefinitions.ToListAsync(ct);

    public async Task AddAsync(WorkflowDefinition entity, CancellationToken ct = default)
        => await _ctx.WorkflowDefinitions.AddAsync(entity, ct);

    public void Update(WorkflowDefinition entity) => _ctx.WorkflowDefinitions.Update(entity);

    public void Delete(WorkflowDefinition entity)
    {
        entity.IsDeleted = true;
        _ctx.WorkflowDefinitions.Update(entity);
    }
}
