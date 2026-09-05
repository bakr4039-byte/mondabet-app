using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mondabet.Employee.Application.Commands.CreateDepartment;
using Mondabet.Employee.Application.Commands.DeleteDepartment;
using Mondabet.Employee.Application.Commands.UpdateDepartment;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Queries.GetDepartment;
using Mondabet.Employee.Application.Queries.ListDepartments;
using Mondabet.Shared.Api;

namespace Mondabet.Employee.Api.Endpoints;

public static class DepartmentEndpoints
{
    public static void MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/departments")
            .WithTags("Departments")
            .RequireAuthorization();

        group.MapGet("/", async (
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new ListDepartmentsQuery(), ct);
            return result.ToApiResult(ctx);
        });

        group.MapGet("/{id:guid}", async (
            Guid id, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetDepartmentQuery(id), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/", async (
            [FromBody] DepartmentCreateDto dto,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new CreateDepartmentCommand(dto), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] DepartmentUpdateDto dto,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new UpdateDepartmentCommand(id, dto), ct);
            return result.ToApiResult(ctx);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteDepartmentCommand(id), ct);
            return result.ToApiResult(ctx);
        });
    }
}
