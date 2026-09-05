using MediatR;
using Mondabet.Attendance.Application.Commands.CheckIn;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Attendance.Domain.Entities;
using Mondabet.Attendance.Domain.Services;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Commands.CheckOut;

public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, Result<CheckInDto>>
{
    private readonly IAttendanceRepository _repo;
    private readonly IShiftRepository _shiftRepo;
    private readonly IUnitOfWork _uow;

    public CheckOutCommandHandler(
        IAttendanceRepository repo,
        IShiftRepository shiftRepo,
        IUnitOfWork uow)
    {
        _repo = repo;
        _shiftRepo = shiftRepo;
        _uow = uow;
    }

    public async Task<Result<CheckInDto>> Handle(CheckOutCommand request, CancellationToken ct)
    {
        var record = await _repo.GetOpenCheckInAsync(request.EmployeeId, ct);
        if (record is null)
            return Error.NotFound("AttendanceRecord", request.EmployeeId);

        var bypassesLocationRules = record.IsFieldPunch || record.IsKioskPunch;
        var shift = await _shiftRepo.GetByIdAsync(record.ShiftId, ct);

        AttendanceCheckStatus? status = record.CheckInStatus switch
        {
            AttendanceCheckStatus.FieldMission => AttendanceCheckStatus.FieldMission,
            _ => null,
        };

        var checkOutTime = DateTime.UtcNow;
        int? overtimeMinutes = null;
        int? deductionMinutes = null;

        if (!bypassesLocationRules && shift is not null)
        {
            var nowTod = TimeOnly.FromDateTime(checkOutTime);

            // For a split shift, pick whichever period (morning/afternoon) "now" is closer to
            // so early-departure is judged against that period's own end time, not the
            // overall shift end. Non-split shifts get (StartTime, EndTime) unchanged.
            var (_, periodEnd) = shift.GetActivePeriod(nowTod);

            // Checking out any time before the shift's own end time (i.e. leaving early) is
            // flagged for follow-up/clarification, but — unlike check-in — it is never
            // rejected outright: an employee must always be able to check out.
            status = nowTod < periodEnd
                ? AttendanceCheckStatus.EarlyDeparture
                : AttendanceCheckStatus.Normal;

            // Best-effort minutes for payroll/reporting - reasonable V1 defaults (like
            // PayrollCalculator's), not a confirmed company/labor-law policy. Doesn't handle
            // an overnight (midnight-wrapping) shift correctly; fine for the common same-day
            // case this covers today.
            overtimeMinutes = status == AttendanceCheckStatus.Normal
                ? Math.Max(0, (int)(nowTod.ToTimeSpan() - periodEnd.ToTimeSpan()).TotalMinutes)
                : 0;

            var lateDeduction = 0;
            if (record.CheckInStatus == AttendanceCheckStatus.Late)
            {
                var checkInTod = TimeOnly.FromDateTime(record.CheckInTime);
                var (periodStartAtCheckIn, _) = shift.GetActivePeriod(checkInTod);
                var graceCutoff = periodStartAtCheckIn.AddMinutes(shift.GracePeriodMinutes);
                lateDeduction = Math.Max(0, (int)(checkInTod.ToTimeSpan() - graceCutoff.ToTimeSpan()).TotalMinutes);
            }

            var earlyDeduction = status == AttendanceCheckStatus.EarlyDeparture
                ? Math.Max(0, (int)(periodEnd.ToTimeSpan() - nowTod.ToTimeSpan()).TotalMinutes)
                : 0;

            deductionMinutes = lateDeduction + earlyDeduction;
        }

        var workDurationMinutes = (int)(checkOutTime - record.CheckInTime).TotalMinutes;

        // Geofence is informational on checkout (never blocks it — an employee must always be
        // able to check out), so an outside-fence checkout is only noted on the audit trail.
        string? note = null;
        if (!bypassesLocationRules && shift is not null
            && request.Latitude.HasValue && request.Longitude.HasValue)
        {
            var isWithin = GeofenceService.IsWithinGeofence(
                request.Latitude.Value, request.Longitude.Value,
                shift.Latitude, shift.Longitude, shift.RadiusMeters);
            if (!isWithin) note = "Checked out outside the shift's geofence.";
        }

        record.CheckOut(request.Latitude, request.Longitude, status, note,
            workDurationMinutes, overtimeMinutes, deductionMinutes);
        _repo.Update(record);
        await _uow.SaveChangesAsync(ct);

        return CheckInCommandHandler.ToDto(record);
    }
}
