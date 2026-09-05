namespace Mondabet.Report.Application.Interfaces;

/// <summary>
/// Mirrors Mondabet.Attendance.Domain.Entities.AttendanceCheckStatus's underlying int values.
/// Report can't reference Attendance's assembly (separate microservice/deployable), and the
/// JSON the Attendance API returns serializes the enum as its plain number (no
/// JsonStringEnumConverter is configured there), so this has to line up by value, not by name.
/// </summary>
public enum PayrollCheckStatus
{
    OnTime = 0,
    Late = 1,
    OutOfFenceRejected = 2,
    OutOfShiftRejected = 3,
    EarlyDeparture = 4,
    Normal = 5,
    FieldMission = 6,
    NotScheduledToday = 7,
}

public record PayrollEmployeeInfo(
    Guid Id,
    string FullNameEn,
    string FullNameAr,
    string? EmployeeNumber,
    Guid DepartmentId,
    Guid? ShiftId,
    decimal? BaseSalary,
    decimal? HourlyRate);

public record PayrollDepartmentInfo(Guid Id, string Name);

public record PayrollShiftInfo(Guid Id, TimeOnly StartTime, TimeOnly EndTime, string DaysOfWeekJson);

public record PayrollCheckInInfo(
    Guid EmployeeId,
    Guid ShiftId,
    DateTime CheckInTime,
    DateTime? CheckOutTime,
    PayrollCheckStatus CheckInStatus);

public interface IPayrollDataService
{
    Task<IReadOnlyList<PayrollEmployeeInfo>> GetEmployeesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PayrollDepartmentInfo>> GetDepartmentsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PayrollShiftInfo>> GetShiftsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PayrollCheckInInfo>> GetCheckInsAsync(
        DateTime from, DateTime to, CancellationToken ct = default);
}
