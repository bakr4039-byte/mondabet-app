using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Report.Application.Queries.AttendanceReport;
using Mondabet.Report.Application.Queries.CompanyReport;

namespace Mondabet.Report.Api.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this WebApplication app)
    {
        // Attendance report for single employee
        app.MapGet("/api/v1/reports/attendance", async (
            Guid? employeeId,
            DateTime from, DateTime to,
            string format,
            IMediator m, CancellationToken ct) =>
        {
            var fmt = format?.ToLowerInvariant() == "pdf" ? ReportFormat.Pdf : ReportFormat.Xlsx;
            var result = await m.Send(
                new AttendanceReportQuery(new AttendanceReportRequest(employeeId, from, to, fmt)), ct);

            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            var f = result.Value!;
            return Results.File(f.Content, f.ContentType, f.FileName);
        }).RequireAuthorization("CompanyAdmin");

        // Company-level report (SuperAdmin only)
        app.MapGet("/api/v1/reports/companies", async (
            string format, IMediator m, CancellationToken ct) =>
        {
            var fmt = format?.ToLowerInvariant() == "pdf" ? ReportFormat.Pdf : ReportFormat.Xlsx;
            var result = await m.Send(
                new CompanyReportQuery(new CompanyReportRequest(fmt)), ct);

            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            var f = result.Value!;
            return Results.File(f.Content, f.ContentType, f.FileName);
        }).RequireAuthorization("SuperAdmin");
    }
}
