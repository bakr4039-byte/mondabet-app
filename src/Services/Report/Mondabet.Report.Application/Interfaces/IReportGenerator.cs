using Mondabet.Report.Application.Interfaces;

namespace Mondabet.Report.Application.Interfaces;

public interface IReportGenerator
{
    byte[] GenerateAttendancePdf(IReadOnlyList<AttendanceRow> rows, DateTime from, DateTime to);
    byte[] GenerateAttendanceExcel(IReadOnlyList<AttendanceRow> rows, DateTime from, DateTime to);
    byte[] GenerateCompanyPdf(IReadOnlyList<TenantSummaryRow> rows);
    byte[] GenerateCompanyExcel(IReadOnlyList<TenantSummaryRow> rows);
}
