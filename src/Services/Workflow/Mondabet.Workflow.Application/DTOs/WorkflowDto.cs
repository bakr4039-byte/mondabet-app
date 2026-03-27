using Mondabet.Workflow.Domain.Entities;

namespace Mondabet.Workflow.Application.DTOs;

public record WorkflowDefinitionDto(
    Guid Id,
    string Name,
    WorkflowAppliesTo AppliesTo,
    string StepsJson);

public record WorkflowDefinitionCreateDto(
    string Name,
    WorkflowAppliesTo AppliesTo,
    string StepsJson);

public record WorkflowDefinitionUpdateDto(
    string Name,
    WorkflowAppliesTo AppliesTo,
    string StepsJson);

public record WorkflowInstanceDto(
    Guid Id,
    Guid DefinitionId,
    Guid EntityId,
    int CurrentStep,
    int TotalSteps,
    WorkflowInstanceStatus Status,
    DateTime CreatedAt);

public record StartWorkflowDto(Guid DefinitionId, Guid EntityId);
public record AdvanceWorkflowDto(bool Approved, string? Comment = null);
