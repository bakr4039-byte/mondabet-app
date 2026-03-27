using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.DTOs;

namespace Mondabet.Workflow.Application.Queries.ListWorkflows;

public record ListWorkflowsQuery : IRequest<Result<IReadOnlyList<WorkflowDefinitionDto>>>;
