using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Queries.ListShifts;

public record ListShiftsQuery : IRequest<Result<IReadOnlyList<ShiftDto>>>;
