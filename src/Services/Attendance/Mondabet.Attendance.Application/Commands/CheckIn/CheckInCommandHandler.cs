using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Attendance.Domain.Entities;
using Mondabet.Attendance.Domain.Services;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Commands.CheckIn;

public class CheckInCommandHandler : IRequestHandler<CheckInCommand, Result<CheckInDto>>
{
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IShiftRepository _shiftRepo;
    private readonly IUnitOfWork _uow;

    public CheckInCommandHandler(
        IAttendanceRepository attendanceRepo,
        IShiftRepository shiftRepo,
        IUnitOfWork uow)
    {
        _attendanceRepo = attendanceRepo;
        _shiftRepo = shiftRepo;
        _uow = uow;
    }

    public async Task<Result<CheckInDto>> Handle(CheckInCommand request, CancellationToken ct)
    {
        var d = request.Dto;

        var shift = await _shiftRepo.GetByIdAsync(d.ShiftId, ct);
        if (shift is null) return Error.NotFound("Shift", d.ShiftId);

        if (await _attendanceRepo.HasCheckInTodayAsync(request.EmployeeId, d.ShiftId, ct))
            return Error.Conflict("Already checked in for this shift today.");

        // Field visits and kiosk terminals are inherently away from (or a shared substitute
        // for) the employee's own GPS position, so geofence + shift-window rules don't apply
        // to them the way they do to a normal GPS check-in.
        var bypassesLocationRules = d.IsFieldPunch || d.IsKioskPunch;

        var now = DateTime.UtcNow;

        if (!bypassesLocationRules && !shift.IsScheduledOn(now.DayOfWeek))
        {
            var rejected = AttendanceRecord.Create(
                request.EmployeeId, d.ShiftId, d.Latitude, d.Longitude,
                isWithinGeofence: false, deviceId: d.DeviceId,
                checkInStatus: AttendanceCheckStatus.NotScheduledToday);
            await _attendanceRepo.AddAsync(rejected, ct);
            await _uow.SaveChangesAsync(ct);
            return Error.Validation("This shift is not scheduled for today.");
        }

        var isWithin = bypassesLocationRules || GeofenceService.IsWithinGeofence(
            d.Latitude, d.Longitude,
            shift.Latitude, shift.Longitude,
            shift.RadiusMeters);

        if (!isWithin)
        {
            var rejected = AttendanceRecord.Create(
                request.EmployeeId, d.ShiftId, d.Latitude, d.Longitude,
                isWithinGeofence: false, deviceId: d.DeviceId,
                checkInStatus: AttendanceCheckStatus.OutOfFenceRejected);
            await _attendanceRepo.AddAsync(rejected, ct);
            await _uow.SaveChangesAsync(ct);
            return Error.Validation("Check-in rejected: outside the allowed geofence.");
        }

        AttendanceCheckStatus status;
        if (bypassesLocationRules)
        {
            status = d.IsFieldPunch ? AttendanceCheckStatus.FieldMission : AttendanceCheckStatus.Normal;
        }
        else
        {
            var nowTod = TimeOnly.FromDateTime(now);
            var windowStart = shift.StartTime.AddMinutes(-shift.WindowStartMinutes);
            var windowEnd = shift.EndTime.AddMinutes(shift.WindowEndMinutes);
            var withinWindow = IsTimeOfDayInRange(nowTod, windowStart, windowEnd);

            if (!withinWindow)
            {
                var rejected = AttendanceRecord.Create(
                    request.EmployeeId, d.ShiftId, d.Latitude, d.Longitude,
                    isWithinGeofence: true, deviceId: d.DeviceId,
                    checkInStatus: AttendanceCheckStatus.OutOfShiftRejected);
                await _attendanceRepo.AddAsync(rejected, ct);
                await _uow.SaveChangesAsync(ct);
                return Error.Validation("Check-in rejected: outside the shift's allowed time window.");
            }

            var onTimeCutoff = shift.StartTime.AddMinutes(shift.GracePeriodMinutes);
            status = IsTimeOfDayInRange(nowTod, shift.StartTime.AddMinutes(-shift.WindowStartMinutes), onTimeCutoff)
                ? AttendanceCheckStatus.OnTime
                : AttendanceCheckStatus.Late;
        }

        var record = AttendanceRecord.Create(
            request.EmployeeId, d.ShiftId,
            d.Latitude, d.Longitude,
            isWithin, d.DeviceId,
            checkInStatus: status,
            facialVerified: d.FacialVerified,
            selfiePhoto: d.SelfiePhoto,
            antiSpoofingScore: d.AntiSpoofingScore,
            isFieldPunch: d.IsFieldPunch,
            fieldClientName: d.FieldClientName,
            isKioskPunch: d.IsKioskPunch,
            kioskTerminalId: d.KioskTerminalId);

        await _attendanceRepo.AddAsync(record, ct);
        await _uow.SaveChangesAsync(ct);

        return ToDto(record);
    }

    /// <summary>
    /// True if <paramref name="value"/> falls within [start, end], where end may wrap past
    /// midnight (e.g. a night shift window from 22:00 to 06:00).
    /// </summary>
    internal static bool IsTimeOfDayInRange(TimeOnly value, TimeOnly start, TimeOnly end)
        => start <= end
            ? value >= start && value <= end
            : value >= start || value <= end;

    internal static CheckInDto ToDto(AttendanceRecord r) => new(
        r.Id, r.EmployeeId, r.ShiftId, r.CheckInTime, r.CheckOutTime,
        r.CheckInLat, r.CheckInLng, r.CheckOutLat, r.CheckOutLng,
        r.IsWithinGeofence, r.DeviceId,
        r.CheckInStatus, r.CheckOutStatus,
        r.FacialVerified, r.IsFieldPunch, r.FieldClientName,
        r.IsKioskPunch, r.KioskTerminalId);
}
