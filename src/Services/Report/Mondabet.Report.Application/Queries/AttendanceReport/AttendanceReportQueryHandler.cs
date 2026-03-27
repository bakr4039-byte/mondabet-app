using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Report.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Report.Application.Queries.AttendanceReport;

public class AttendanceReportQueryHandler
    : IRequestHandler<AttendanceReportQuery, Result<ReportFileDto>>
{
    private readonly IAttendanceDataService _data;
    private readonly IReportGenerator _generator;

    public AttendanceReportQueryHandler(
        IAttendanceDataService data, IReportGenerator generator)
    {
        _data = data;
        _generator = generator;
    }

    public async Task<Result<ReportFileDto>> Handle(
        AttendanceReportQuery request, CancellationToken ct)
    {
        var r = request.Request;
        var rows = await _data.GetAttendanceRowsAsync(r.EmployeeId, r.From, r.To, ct);

        var (content, contentType, extension) = r.Format switch
        {
            ReportFormat.Pdf => (
                _generator.GenerateAttendancePdf(rows, r.From, r.To),
                "application/pdf",
                "pdf"),
            _ => (
                _generator.GenerateAttendanceExcel(rows, r.From, r.To),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "xlsx"),
        };

        var scope = r.EmployeeId.HasValue ? $"employee_{r.EmployeeId}" : "all";
        var fileName = $"attendance_{scope}_{r.From:yyyyMMdd}_{r.To:yyyyMMdd}.{extension}";

        return new ReportFileDto(content, contentType, fileName);
    }
}
