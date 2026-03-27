using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.DTOs;
using Mondabet.Workflow.Application.Interfaces;
using Mondabet.Workflow.Domain.Entities;

namespace Mondabet.Workflow.Application.Commands.CreateWorkflow;

public class CreateWorkflowCommandHandler
    : IRequestHandler<CreateWorkflowCommand, Result<WorkflowDefinitionDto>>
{
    private readonly IWorkflowDefinitionRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateWorkflowCommandHandler(IWorkflowDefinitionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkflowDefinitionDto>> Handle(
        CreateWorkflowCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var def = WorkflowDefinition.Create(d.Name, d.AppliesTo, d.StepsJson);
        await _repo.AddAsync(def, ct);
        await _uow.SaveChangesAsync(ct);
        return ToDto(def);
    }

    internal static WorkflowDefinitionDto ToDto(WorkflowDefinition d) => new(
        d.Id, d.Name, d.AppliesTo, d.StepsJson);
}
