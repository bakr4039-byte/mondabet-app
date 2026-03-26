using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Commands.CheckOut;

public record CheckOutCommand(Guid EmployeeId) : IRequest<Result<CheckInDto>>;
