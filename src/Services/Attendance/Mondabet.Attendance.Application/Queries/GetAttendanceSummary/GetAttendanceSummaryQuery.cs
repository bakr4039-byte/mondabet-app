using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Queries.GetAttendanceSummary;

public record GetAttendanceSummaryQuery(
    Guid EmployeeId,
    DateTime From,
    DateTime To) : IRequest<Result<AttendanceSummaryDto>>;
