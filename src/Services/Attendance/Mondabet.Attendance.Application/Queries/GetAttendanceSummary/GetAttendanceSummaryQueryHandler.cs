using MediatR;
using Mondabet.Attendance.Application.Commands.CheckIn;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Queries.GetAttendanceSummary;

public class GetAttendanceSummaryQueryHandler
    : IRequestHandler<GetAttendanceSummaryQuery, Result<AttendanceSummaryDto>>
{
    private readonly IAttendanceRepository _repo;

    public GetAttendanceSummaryQueryHandler(IAttendanceRepository repo) => _repo = repo;

    public async Task<Result<AttendanceSummaryDto>> Handle(
        GetAttendanceSummaryQuery request, CancellationToken ct)
    {
        var records = await _repo.GetByEmployeeAsync(
            request.EmployeeId, request.From, request.To, ct);

        var totalDays = (int)(request.To.Date - request.From.Date).TotalDays + 1;
        var presentDays = records.Select(r => r.CheckInTime.Date).Distinct().Count();

        // Late = checked in after shift start time (simplified: flag if CheckInTime > scheduled start)
        // Full implementation requires joining with Shift.StartTime — simplified here
        var lateDays = 0;
        var absentDays = Math.Max(0, totalDays - presentDays);

        var dtos = records.Select(CheckInCommandHandler.ToDto).ToList();

        return new AttendanceSummaryDto(
            request.EmployeeId, totalDays, presentDays, lateDays, absentDays, dtos);
    }
}
