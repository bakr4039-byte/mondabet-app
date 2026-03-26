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

        var isWithin = GeofenceService.IsWithinGeofence(
            d.Latitude, d.Longitude,
            shift.Latitude, shift.Longitude,
            shift.RadiusMeters);

        var record = AttendanceRecord.Create(
            request.EmployeeId, d.ShiftId,
            d.Latitude, d.Longitude,
            isWithin, d.DeviceId);

        await _attendanceRepo.AddAsync(record, ct);
        await _uow.SaveChangesAsync(ct);

        return ToDto(record);
    }

    internal static CheckInDto ToDto(AttendanceRecord r) => new(
        r.Id, r.EmployeeId, r.ShiftId, r.CheckInTime, r.CheckOutTime,
        r.CheckInLat, r.CheckInLng, r.IsWithinGeofence, r.DeviceId);
}
