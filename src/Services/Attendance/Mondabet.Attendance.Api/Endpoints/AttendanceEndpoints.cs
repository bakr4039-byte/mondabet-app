using MediatR;
using Mondabet.Attendance.Application.Commands.CheckIn;
using Mondabet.Attendance.Application.Commands.CheckOut;
using Mondabet.Attendance.Application.Commands.CreateShift;
using Mondabet.Attendance.Application.Commands.UpdateShift;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Queries.GetAttendanceSummary;
using Mondabet.Attendance.Application.Queries.GetCurrentShift;
using Mondabet.Attendance.Application.Queries.ListCheckIns;
using Mondabet.Attendance.Application.Queries.ListShifts;

namespace Mondabet.Attendance.Api.Endpoints;

public static class AttendanceEndpoints
{
    public static void MapAttendanceEndpoints(this WebApplication app)
    {
        var shifts = app.MapGroup("/api/v1/shifts").RequireAuthorization("CompanyAdmin");
        var checkins = app.MapGroup("/api/v1/checkins").RequireAuthorization();

        // --- Shifts ---
        shifts.MapGet("/", async (IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListShiftsQuery(), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        });

        shifts.MapPost("/", async (ShiftCreateDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CreateShiftCommand(dto), ct);
            return result.IsSuccess
                ? Results.Created($"/api/v1/shifts/{result.Value!.Id}", result.Value)
                : Results.BadRequest(result.Error);
        });

        shifts.MapPut("/{id:guid}", async (Guid id, ShiftUpdateDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdateShiftCommand(id, dto), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        });

        // Self-scoped: any authenticated employee (not just CompanyAdmin) resolving their own
        // assigned shift for the mobile check-in screen. Mapped outside the "shifts" group
        // (which requires CompanyAdmin) so a plain Employee token can call it.
        app.MapGet("/api/v1/shifts/current", async (
            IMediator m, HttpContext ctx, CancellationToken ct) =>
        {
            var employeeId = GetEmployeeId(ctx);
            if (employeeId == Guid.Empty) return Results.Unauthorized();

            var result = await m.Send(new GetCurrentShiftQuery(employeeId), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        }).RequireAuthorization();

        // --- Check-ins ---
        checkins.MapPost("/", async (
            CheckInRequestDto dto, IMediator m,
            HttpContext ctx, CancellationToken ct) =>
        {
            var employeeId = GetEmployeeId(ctx);
            if (employeeId == Guid.Empty) return Results.Unauthorized();

            var result = await m.Send(new CheckInCommand(employeeId, dto), ct);
            return result.IsSuccess
                ? Results.Created($"/api/v1/checkins/{result.Value!.Id}", result.Value)
                : MapError(result.Error!);
        });

        checkins.MapPost("/checkout", async (
            double? lat, double? lng,
            IMediator m, HttpContext ctx, CancellationToken ct) =>
        {
            var employeeId = GetEmployeeId(ctx);
            if (employeeId == Guid.Empty) return Results.Unauthorized();

            var result = await m.Send(new CheckOutCommand(employeeId, lat, lng), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        });

        checkins.MapGet("/", async (
            Guid? employeeId, DateTime? from, DateTime? to,
            int page, int size, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(
                new ListCheckInsQuery(employeeId, from, to, page == 0 ? 1 : page, size == 0 ? 20 : size),
                ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }).RequireAuthorization("CompanyAdmin");

        // Self-scoped: the caller's own attendance history for the mobile app - reuses the same
        // paged query as the CompanyAdmin list above, but forces EmployeeId to the caller so no
        // elevated role is required. Inherits the "checkins" group's bare RequireAuthorization().
        checkins.MapGet("/my", async (
            DateTime? from, DateTime? to, int page, int size,
            IMediator m, HttpContext ctx, CancellationToken ct) =>
        {
            var employeeId = GetEmployeeId(ctx);
            if (employeeId == Guid.Empty) return Results.Unauthorized();

            var result = await m.Send(
                new ListCheckInsQuery(employeeId, from, to, page == 0 ? 1 : page, size == 0 ? 20 : size),
                ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        // Attendance summary per employee
        app.MapGet("/api/v1/attendance/employee/{id:guid}", async (
            Guid id, DateTime from, DateTime to, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetAttendanceSummaryQuery(id, from, to), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
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
