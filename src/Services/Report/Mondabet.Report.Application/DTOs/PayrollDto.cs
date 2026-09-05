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

/// Per-employee punctuality summary over a date range - surfaces data that already existed
/// on the Employee entity as dead fields (PunctualityScore/ConsecutiveOnTimeDays/
/// GamificationBadge were settable but nothing ever computed or displayed them). Computed
/// on demand from the same check-in data the payroll report already fetches, rather than
/// persisted back onto the Employee record, so this stays a pure read - no new writes,
/// migrations, or background jobs.
public record EmployeePunctualityDto(
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeNameAr,
    int TotalCheckIns,
    int OnTimeCount,
    int LateCount,
    double PunctualityScore,
    int ConsecutiveOnTimeDays,
    string Badge);
