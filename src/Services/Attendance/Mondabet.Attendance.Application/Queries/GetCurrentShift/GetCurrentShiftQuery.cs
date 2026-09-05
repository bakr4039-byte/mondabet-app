using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Queries.GetCurrentShift;

public record GetCurrentShiftQuery(Guid EmployeeId) : IRequest<Result<ShiftDto>>;
