namespace Mondabet.Attendance.Application.DTOs;

public record CheckInDto(
    Guid Id,
    Guid EmployeeId,
    Guid ShiftId,
    DateTime CheckInTime,
    DateTime? CheckOutTime,
    double CheckInLat,
    double CheckInLng,
    bool IsWithinGeofence,
    string DeviceId);

public record CheckInRequestDto(
    Guid ShiftId,
    double Latitude,
    double Longitude,
    string DeviceId);

public record AttendanceSummaryDto(
    Guid EmployeeId,
    int TotalDays,
    int PresentDays,
    int LateDays,
    int AbsentDays,
    IReadOnlyList<CheckInDto> Records);
