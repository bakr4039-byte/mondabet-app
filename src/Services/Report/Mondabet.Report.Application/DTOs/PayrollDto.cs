namespace Mondabet.Report.Application.DTOs;

public record PayrollSummaryDto(
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeNameAr,
    string? EmployeeNumber,
    string DepartmentName,
    decimal BaseSalary,
    int WorkingDays,
    int PresentDays,
    int LateDays,
    int TotalLateMinutes,
    double TotalWorkHours,
    double TotalOvertimeHours,
    decimal OvertimePay,
    decimal LateDeduction,
    decimal NetPayableSalary,
    string Currency);

public record PayrollReportRequest(int Year, int Month, ReportFormat Format);
