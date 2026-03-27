using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.DTOs;

namespace Mondabet.Workflow.Application.Commands.AdvanceWorkflow;

public record AdvanceWorkflowCommand(Guid InstanceId, AdvanceWorkflowDto Dto)
    : IRequest<Result<WorkflowInstanceDto>>;
