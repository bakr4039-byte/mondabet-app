using Mondabet.Report.Application.DTOs;

namespace Mondabet.Report.Application.Interfaces;

public interface IReportGenerator
{
    byte[] GenerateAttendancePdf(IReadOnlyList<AttendanceRow> rows, DateTime from, DateTime to);
    byte[] GenerateAttendanceExcel(IReadOnlyList<AttendanceRow> rows, DateTime from, DateTime to);
    byte[] GenerateCompanyPdf(IReadOnlyList<TenantSummaryRow> rows);
    byte[] GenerateCompanyExcel(IReadOnlyList<TenantSummaryRow> rows);
    byte[] GeneratePayrollPdf(IReadOnlyList<PayrollSummaryDto> rows, int year, int month);
    byte[] GeneratePayrollExcel(IReadOnlyList<PayrollSummaryDto> rows, int year, int month);
}
