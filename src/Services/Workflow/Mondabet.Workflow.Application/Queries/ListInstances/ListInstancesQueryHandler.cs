using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.Commands.StartWorkflow;
using Mondabet.Workflow.Application.DTOs;
using Mondabet.Workflow.Application.Interfaces;

namespace Mondabet.Workflow.Application.Queries.ListInstances;

public class ListInstancesQueryHandler
    : IRequestHandler<ListInstancesQuery, Result<IReadOnlyList<WorkflowInstanceDto>>>
{
    private readonly IWorkflowInstanceRepository _repo;
    public ListInstancesQueryHandler(IWorkflowInstanceRepository repo) => _repo = repo;

    public async Task<Result<IReadOnlyList<WorkflowInstanceDto>>> Handle(
        ListInstancesQuery request, CancellationToken ct)
    {
        var instances = await _repo.GetByDefinitionAsync(request.DefinitionId, ct);
        return instances.Select(StartWorkflowCommandHandler.ToDto).ToList();
    }
}
