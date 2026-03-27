using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.DTOs;

namespace Mondabet.Workflow.Application.Commands.UpdateWorkflow;

public record UpdateWorkflowCommand(Guid Id, WorkflowDefinitionUpdateDto Dto)
    : IRequest<Result<WorkflowDefinitionDto>>;
