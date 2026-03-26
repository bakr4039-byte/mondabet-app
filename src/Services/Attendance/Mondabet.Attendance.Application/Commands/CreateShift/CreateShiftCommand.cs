using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Commands.CreateShift;

public record CreateShiftCommand(ShiftCreateDto Dto) : IRequest<Result<ShiftDto>>;
