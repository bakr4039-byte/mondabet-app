using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Commands.UpdateShift;

public record UpdateShiftCommand(Guid ShiftId, ShiftUpdateDto Dto) : IRequest<Result<ShiftDto>>;
