using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.DTOs;

namespace Mondabet.Workflow.Application.Queries.ListInstances;

public record ListInstancesQuery(Guid DefinitionId)
    : IRequest<Result<IReadOnlyList<WorkflowInstanceDto>>>;
