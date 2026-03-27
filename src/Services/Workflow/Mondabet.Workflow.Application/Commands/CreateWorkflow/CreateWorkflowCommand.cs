using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.DTOs;

namespace Mondabet.Workflow.Application.Commands.CreateWorkflow;

public record CreateWorkflowCommand(WorkflowDefinitionCreateDto Dto)
    : IRequest<Result<WorkflowDefinitionDto>>;
