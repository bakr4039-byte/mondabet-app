using MediatR;
using Mondabet.Clarification.Application.Commands.CreateClarification;
using Mondabet.Clarification.Application.Commands.RespondClarification;
using Mondabet.Clarification.Application.DTOs;
using Mondabet.Clarification.Application.Queries.ListClarifications;

namespace Mondabet.Clarification.Api.Endpoints;

public static class ClarificationEndpoints
{
    public static void MapClarificationEndpoints(this WebApplication app)
    {
        var clarifications = app.MapGroup("/api/v1/clarifications")
            .RequireAuthorization();

        // Employee: submit a clarification request
        clarifications.MapPost("/", async (
            CreateClarificationDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CreateClarificationCommand(dto), ct);
            return result.IsSuccess
                ? Results.Created($"/api/v1/clarifications/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Error);
        });

        // Employee: list own clarifications
        clarifications.MapGet("/employee/{employeeId:guid}", async (
            Guid employeeId, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListClarificationsQuery(employeeId), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        // Manager: respond to a clarification
        clarifications.MapPut("/{id:guid}/respond", async (
            Guid id, RespondClarificationDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new RespondClarificationCommand(id, dto), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : MapError(result.Error!);
        }).RequireAuthorization("CompanyAdmin");
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
