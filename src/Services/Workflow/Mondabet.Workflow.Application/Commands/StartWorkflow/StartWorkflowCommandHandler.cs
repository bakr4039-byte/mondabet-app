using System.Text.Json;
using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Workflow.Application.DTOs;
using Mondabet.Workflow.Application.Interfaces;
using Mondabet.Workflow.Domain.Entities;

namespace Mondabet.Workflow.Application.Commands.StartWorkflow;

public class StartWorkflowCommandHandler
    : IRequestHandler<StartWorkflowCommand, Result<WorkflowInstanceDto>>
{
    private readonly IWorkflowDefinitionRepository _defRepo;
    private readonly IWorkflowInstanceRepository _instanceRepo;
    private readonly IUnitOfWork _uow;

    public StartWorkflowCommandHandler(
        IWorkflowDefinitionRepository defRepo,
        IWorkflowInstanceRepository instanceRepo,
        IUnitOfWork uow)
    {
        _defRepo = defRepo;
        _instanceRepo = instanceRepo;
        _uow = uow;
    }

    public async Task<Result<WorkflowInstanceDto>> Handle(
        StartWorkflowCommand request, CancellationToken ct)
    {
        var def = await _defRepo.GetByIdAsync(request.Dto.DefinitionId, ct);
        if (def is null) return Error.NotFound("WorkflowDefinition", request.Dto.DefinitionId);

        // Count steps from JSON
        var steps = JsonSerializer.Deserialize<JsonElement[]>(def.StepsJson);
        int totalSteps = steps?.Length ?? 1;

        var instance = WorkflowInstance.Create(
            request.Dto.DefinitionId, request.Dto.EntityId, totalSteps);

        await _instanceRepo.AddAsync(instance, ct);
        await _uow.SaveChangesAsync(ct);

        return ToDto(instance);
    }

    internal static WorkflowInstanceDto ToDto(WorkflowInstance i) => new(
        i.Id, i.DefinitionId, i.EntityId,
        i.CurrentStep, i.TotalSteps, i.Status, i.CreatedAt);
}
