using MassTransit;
using MediatR;
using Mondabet.Workflow.Application.Commands.StartWorkflow;
using Mondabet.Workflow.Application.DTOs;
using Mondabet.Workflow.Application.Interfaces;
using Mondabet.Workflow.Domain.Entities;

namespace Mondabet.Workflow.Infrastructure.Consumers;

/// <summary>
/// Message contract published by leave-svc when a leave request is submitted.
/// workflow-svc consumes it and starts the appropriate workflow instance.
/// </summary>
public record LeaveRequestedMessage(
    Guid LeaveRequestId,
    Guid TenantId,
    int LeaveType);   // 1=Vacation 2=Permission

public class LeaveRequestedConsumer : IConsumer<LeaveRequestedMessage>
{
    private readonly IWorkflowDefinitionRepository _defRepo;
    private readonly IMediator _mediator;

    public LeaveRequestedConsumer(
        IWorkflowDefinitionRepository defRepo,
        IMediator mediator)
    {
        _defRepo = defRepo;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<LeaveRequestedMessage> context)
    {
        var msg = context.Message;

        // Find matching workflow definition for this tenant + leave type
        var appliesTo = msg.LeaveType switch
        {
            1 => WorkflowAppliesTo.Vacation,
            2 => WorkflowAppliesTo.Permission,
            _ => WorkflowAppliesTo.Both,
        };

        var defs = await _defRepo.GetAllAsync(context.CancellationToken);
        var def = defs.FirstOrDefault(d =>
            d.AppliesTo == appliesTo || d.AppliesTo == WorkflowAppliesTo.Both);

        if (def is null) return; // No workflow configured — leave is auto-approved

        await _mediator.Send(new StartWorkflowCommand(
            new StartWorkflowDto(def.Id, msg.LeaveRequestId)),
            context.CancellationToken);
    }
}
