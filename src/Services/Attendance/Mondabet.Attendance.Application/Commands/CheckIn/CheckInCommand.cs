using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Commands.CheckIn;

public record CheckInCommand(Guid EmployeeId, CheckInRequestDto Dto) : IRequest<Result<CheckInDto>>;
