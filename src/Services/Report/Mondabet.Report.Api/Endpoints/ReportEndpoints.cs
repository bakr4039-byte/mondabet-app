using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Report.Application.Queries.AttendanceReport;
using Mondabet.Report.Application.Queries.CompanyReport;
using Mondabet.Report.Application.Queries.PayrollReport;
using Mondabet.Report.Application.Queries.PunctualityAnalytics;

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

        // Payroll report for a given month (CompanyAdmin — scoped to their own tenant)
        app.MapGet("/api/v1/reports/payroll", async (
            int year, int month,
            string format,
            IMediator m, CancellationToken ct) =>
        {
            var fmt = format?.ToLowerInvariant() == "pdf" ? ReportFormat.Pdf : ReportFormat.Xlsx;
            var result = await m.Send(
                new PayrollReportQuery(new PayrollReportRequest(year, month, fmt)), ct);

            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            var f = result.Value!;
            return Results.File(f.Content, f.ContentType, f.FileName);
        }).RequireAuthorization("CompanyAdmin");

        // Punctuality/discipline analytics dashboard - plain JSON (not a file download like the
        // reports above), computed on demand from existing check-in data. Surfaces the
        // PunctualityScore/ConsecutiveOnTimeDays/GamificationBadge concept that already existed
        // as dead fields on Employee but was never actually calculated or shown anywhere.
        app.MapGet("/api/v1/reports/punctuality", async (
            DateTime from, DateTime to, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new PunctualityAnalyticsQuery(from, to), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }).RequireAuthorization("CompanyAdmin");
    }
}
