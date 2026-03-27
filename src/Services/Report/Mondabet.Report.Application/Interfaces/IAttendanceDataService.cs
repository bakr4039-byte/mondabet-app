namespace Mondabet.Report.Application.Interfaces;

public record AttendanceRow(
    string EmployeeName,
    string Iqama,
    DateTime CheckInTime,
    DateTime? CheckOutTime,
    bool IsWithinGeofence,
    string ShiftName);

public record TenantSummaryRow(
    string TenantCode,
    string CompanyName,
    int EmployeeCount,
    DateTime? LastActivity);

public interface IAttendanceDataService
{
    Task<IReadOnlyList<AttendanceRow>> GetAttendanceRowsAsync(
        Guid? employeeId, DateTime from, DateTime to, CancellationToken ct = default);
}

public interface ITenantDataService
{
    Task<IReadOnlyList<TenantSummaryRow>> GetTenantSummaryRowsAsync(CancellationToken ct = default);
}
