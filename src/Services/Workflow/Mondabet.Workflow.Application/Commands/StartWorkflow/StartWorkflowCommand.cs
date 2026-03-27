using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.DTOs;

namespace Mondabet.Workflow.Application.Commands.StartWorkflow;

public record StartWorkflowCommand(StartWorkflowDto Dto) : IRequest<Result<WorkflowInstanceDto>>;
