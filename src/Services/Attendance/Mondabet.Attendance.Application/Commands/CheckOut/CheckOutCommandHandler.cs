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

        if (!bypassesLocationRules && shift is not null)
        {
            var now = DateTime.UtcNow;
            var nowTod = TimeOnly.FromDateTime(now);

            // Checking out any time before the shift's own end time (i.e. leaving early) is
            // flagged for follow-up/clarification, but — unlike check-in — it is never
            // rejected outright: an employee must always be able to check out.
            status = nowTod < shift.EndTime
                ? AttendanceCheckStatus.EarlyDeparture
                : AttendanceCheckStatus.Normal;
        }

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

        record.CheckOut(request.Latitude, request.Longitude, status, note);
        _repo.Update(record);
        await _uow.SaveChangesAsync(ct);

        return CheckInCommandHandler.ToDto(record);
    }
}
