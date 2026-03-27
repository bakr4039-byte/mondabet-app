using MediatR;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Commands.ApproveLeave;

public record ApproveLeaveCommand(Guid LeaveId, ApproveLeaveDto Dto) : IRequest<Result<LeaveDto>>;
