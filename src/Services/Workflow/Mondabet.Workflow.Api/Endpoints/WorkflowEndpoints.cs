using MediatR;
using Mondabet.Workflow.Application.Commands.AdvanceWorkflow;
using Mondabet.Workflow.Application.Commands.CreateWorkflow;
using Mondabet.Workflow.Application.Commands.StartWorkflow;
using Mondabet.Workflow.Application.Commands.UpdateWorkflow;
using Mondabet.Workflow.Application.DTOs;
using Mondabet.Workflow.Application.Queries.ListInstances;
using Mondabet.Workflow.Application.Queries.ListWorkflows;

namespace Mondabet.Workflow.Api.Endpoints;

public static class WorkflowEndpoints
{
    public static void MapWorkflowEndpoints(this WebApplication app)
    {
        var workflows = app.MapGroup("/api/v1/workflows")
            .RequireAuthorization("CompanyAdmin");

        // Workflow definition CRUD
        workflows.MapGet("/", async (IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListWorkflowsQuery(), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        workflows.MapPost("/", async (
            WorkflowDefinitionCreateDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CreateWorkflowCommand(dto), ct);
            return result.IsSuccess
                ? Results.Created($"/api/v1/workflows/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Error);
        });

        workflows.MapPut("/{id:guid}", async (
            Guid id, WorkflowDefinitionUpdateDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdateWorkflowCommand(id, dto), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        });

        // Workflow instances
        workflows.MapGet("/{id:guid}/instances", async (
            Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListInstancesQuery(id), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        });

        // Start a workflow instance manually (normally triggered by MassTransit consumer)
        workflows.MapPost("/instances", async (
            StartWorkflowDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new StartWorkflowCommand(dto), ct);
            return result.IsSuccess
                ? Results.Created($"/api/v1/workflows/instances/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Error);
        });

        // Advance (approve/reject) a workflow step
        workflows.MapPut("/instances/{id:guid}/advance", async (
            Guid id, AdvanceWorkflowDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new AdvanceWorkflowCommand(id, dto), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : MapError(result.Error!);
        });
    }

    private static IResult MapError(object error)
    {
        var code = error?.GetType().GetProperty("Code")?.GetValue(error)?.ToString() ?? "";
        return code switch
        {
            "NotFound" => Results.NotFound(error),
            "Conflict" => Results.Conflict(error),
            _ => Results.BadRequest(error),
        };
    }
}
