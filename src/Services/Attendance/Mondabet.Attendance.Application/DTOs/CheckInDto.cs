using Mondabet.Attendance.Domain.Entities;

namespace Mondabet.Attendance.Application.DTOs;

public record CheckInDto(
    Guid Id,
    Guid EmployeeId,
    Guid ShiftId,
    DateTime CheckInTime,
    DateTime? CheckOutTime,
    double CheckInLat,
    double CheckInLng,
    double? CheckOutLat,
    double? CheckOutLng,
    bool IsWithinGeofence,
    string DeviceId,
    AttendanceCheckStatus CheckInStatus,
    AttendanceCheckStatus? CheckOutStatus,
    bool FacialVerified,
    bool IsFieldPunch,
    string? FieldClientName,
    bool IsKioskPunch,
    string? KioskTerminalId,
    string? FieldVisitPurpose = null,
    string? FieldProofPhoto = null,
    ShiftPeriod? SplitPeriod = null,
    int? WorkDurationMinutes = null,
    int? OvertimeMinutes = null,
    int? DeductionMinutes = null);

public record CheckInRequestDto(
    Guid ShiftId,
    double Latitude,
    double Longitude,
    string DeviceId,
    bool FacialVerified = false,
    string? SelfiePhoto = null,
    double? AntiSpoofingScore = null,
    bool IsFieldPunch = false,
    string? FieldClientName = null,
    bool IsKioskPunch = false,
    string? KioskTerminalId = null,
    string? FieldVisitPurpose = null,
    string? FieldProofPhoto = null);

public record AttendanceSummaryDto(
    Guid EmployeeId,
    int TotalDays,
    int PresentDays,
    int LateDays,
    int AbsentDays,
    IReadOnlyList<CheckInDto> Records);
