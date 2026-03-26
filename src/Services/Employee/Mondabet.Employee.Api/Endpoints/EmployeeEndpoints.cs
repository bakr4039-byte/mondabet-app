using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mondabet.Employee.Application.Commands.CreateEmployee;
using Mondabet.Employee.Application.Commands.DeleteEmployee;
using Mondabet.Employee.Application.Commands.ImportEmployees;
using Mondabet.Employee.Application.Commands.UpdateEmployee;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Queries.GetEmployee;
using Mondabet.Employee.Application.Queries.ListEmployees;
using Mondabet.Shared.Api;

namespace Mondabet.Employee.Api.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/employees")
            .WithTags("Employees")
            .RequireAuthorization();

        group.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int size,
            [FromQuery] string? search,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new ListEmployeesQuery(page < 1 ? 1 : page, size < 1 ? 20 : size, search), ct);
            return result.ToApiResult(ctx);
        });

        group.MapGet("/{id:guid}", async (
            Guid id, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetEmployeeQuery(id), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/", async (
            [FromBody] EmployeeCreateDto dto,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new CreateEmployeeCommand(dto), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] EmployeeUpdateDto dto,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new UpdateEmployeeCommand(id, dto), ct);
            return result.ToApiResult(ctx);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteEmployeeCommand(id), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/import", async (
            IFormFile file,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            if (file is null || file.Length == 0)
                return Results.BadRequest("Excel file is required.");

            using var stream = file.OpenReadStream();
            var result = await mediator.Send(new ImportEmployeesCommand(stream), ct);
            return result.ToApiResult(ctx);
        }).DisableAntiforgery();
    }
}
