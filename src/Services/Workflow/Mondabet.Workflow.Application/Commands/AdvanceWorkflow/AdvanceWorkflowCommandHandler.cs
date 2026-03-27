using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.Commands.StartWorkflow;
using Mondabet.Workflow.Application.DTOs;
using Mondabet.Workflow.Application.Interfaces;
using Mondabet.Workflow.Domain.Entities;

namespace Mondabet.Workflow.Application.Commands.AdvanceWorkflow;

public class AdvanceWorkflowCommandHandler
    : IRequestHandler<AdvanceWorkflowCommand, Result<WorkflowInstanceDto>>
{
    private readonly IWorkflowInstanceRepository _repo;
    private readonly IUnitOfWork _uow;

    public AdvanceWorkflowCommandHandler(IWorkflowInstanceRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkflowInstanceDto>> Handle(
        AdvanceWorkflowCommand request, CancellationToken ct)
    {
        var instance = await _repo.GetByIdAsync(request.InstanceId, ct);
        if (instance is null) return Error.NotFound("WorkflowInstance", request.InstanceId);

        if (instance.Status != WorkflowInstanceStatus.InProgress)
            return Error.Conflict("Workflow instance is no longer in progress.");

        if (request.Dto.Approved)
            instance.Advance();
        else
            instance.Reject();

        _repo.Update(instance);
        await _uow.SaveChangesAsync(ct);

        return StartWorkflowCommandHandler.ToDto(instance);
    }
}
