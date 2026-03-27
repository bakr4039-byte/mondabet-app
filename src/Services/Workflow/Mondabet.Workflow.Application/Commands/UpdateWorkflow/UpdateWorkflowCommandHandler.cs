using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.Commands.CreateWorkflow;
using Mondabet.Workflow.Application.DTOs;
using Mondabet.Workflow.Application.Interfaces;

namespace Mondabet.Workflow.Application.Commands.UpdateWorkflow;

public class UpdateWorkflowCommandHandler
    : IRequestHandler<UpdateWorkflowCommand, Result<WorkflowDefinitionDto>>
{
    private readonly IWorkflowDefinitionRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateWorkflowCommandHandler(IWorkflowDefinitionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkflowDefinitionDto>> Handle(
        UpdateWorkflowCommand request, CancellationToken ct)
    {
        var def = await _repo.GetByIdAsync(request.Id, ct);
        if (def is null) return Error.NotFound("WorkflowDefinition", request.Id);

        var d = request.Dto;
        def.Update(d.Name, d.AppliesTo, d.StepsJson);
        _repo.Update(def);
        await _uow.SaveChangesAsync(ct);
        return CreateWorkflowCommandHandler.ToDto(def);
    }
}
