using MediatR;
using Mondabet.Attendance.Application.Commands.CheckIn;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Attendance.Domain.Entities;
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

        // Now backed by the real per-record status set at check-in time (grace-period aware),
        // rather than a placeholder — see CheckInCommandHandler.
        var lateDays = records
            .Where(r => r.CheckInStatus == AttendanceCheckStatus.Late)
            .Select(r => r.CheckInTime.Date)
            .Distinct()
            .Count();
        var absentDays = Math.Max(0, totalDays - presentDays);

        var dtos = records.Select(CheckInCommandHandler.ToDto).ToList();

        return new AttendanceSummaryDto(
            request.EmployeeId, totalDays, presentDays, lateDays, absentDays, dtos);
    }
}
