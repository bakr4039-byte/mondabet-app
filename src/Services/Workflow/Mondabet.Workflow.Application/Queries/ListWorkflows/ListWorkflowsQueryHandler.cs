using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.Commands.CreateWorkflow;
using Mondabet.Workflow.Application.DTOs;
using Mondabet.Workflow.Application.Interfaces;

namespace Mondabet.Workflow.Application.Queries.ListWorkflows;

public class ListWorkflowsQueryHandler
    : IRequestHandler<ListWorkflowsQuery, Result<IReadOnlyList<WorkflowDefinitionDto>>>
{
    private readonly IWorkflowDefinitionRepository _repo;
    public ListWorkflowsQueryHandler(IWorkflowDefinitionRepository repo) => _repo = repo;

    public async Task<Result<IReadOnlyList<WorkflowDefinitionDto>>> Handle(
        ListWorkflowsQuery request, CancellationToken ct)
    {
        var defs = await _repo.GetAllAsync(ct);
        return defs.Select(CreateWorkflowCommandHandler.ToDto).ToList();
    }
}
