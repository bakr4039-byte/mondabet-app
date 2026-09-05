using MediatR;
using Mondabet.Leave.Application.Commands.ApproveLeave;
using Mondabet.Leave.Application.Commands.RejectLeave;
using Mondabet.Leave.Application.Commands.SubmitLeave;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Leave.Application.Queries.GetLeave;
using Mondabet.Leave.Application.Queries.GetSubstituteCandidates;
using Mondabet.Leave.Application.Queries.ListLeaves;
using Mondabet.Leave.Domain.Entities;

namespace Mondabet.Leave.Api.Endpoints;

public static class LeaveEndpoints
{
    public static void MapLeaveEndpoints(this WebApplication app)
    {
        var leaves = app.MapGroup("/api/v1/leaves").RequireAuthorization();

        // Employee submits leave
        leaves.MapPost("/", async (
            SubmitLeaveDto dto, IMediator m,
            HttpContext ctx, CancellationToken ct) =>
        {
            var employeeId = GetEmployeeId(ctx);
            if (employeeId == Guid.Empty) return Results.Unauthorized();

            var result = await m.Send(new SubmitLeaveCommand(employeeId, dto), ct);
            return result.IsSuccess
                ? Results.Created($"/api/v1/leaves/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Error);
        });

        // List leaves (employee sees own; admin sees all with filter)
        leaves.MapGet("/", async (
            Guid? employeeId, LeaveType? type, LeaveStatus? status,
            int page, int size, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(
                new ListLeavesQuery(employeeId, type, status,
                    page == 0 ? 1 : page, size == 0 ? 20 : size),
                ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        // Get single leave
        leaves.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetLeaveQuery(id), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        });

        // Approve (CompanyAdmin)
        leaves.MapPut("/{id:guid}/approve", async (
            Guid id, ApproveLeaveDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ApproveLeaveCommand(id, dto), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : MapError(result.Error!);
        }).RequireAuthorization("CompanyAdmin");

        // Reject (CompanyAdmin)
        leaves.MapPut("/{id:guid}/reject", async (
            Guid id, RejectLeaveDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new RejectLeaveCommand(id, dto), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : MapError(result.Error!);
        }).RequireAuthorization("CompanyAdmin");

        // Suggested substitute colleagues for an approved request (CompanyAdmin) - same
        // department + job title, not themselves already approved-off during the same dates.
        leaves.MapGet("/{id:guid}/substitutes", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetSubstituteCandidatesQuery(id), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : MapError(result.Error!);
        }).RequireAuthorization("CompanyAdmin");
    }

    private static Guid GetEmployeeId(HttpContext ctx)
    {
        var sub = ctx.User.FindFirst("sub")?.Value
               ?? ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
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
