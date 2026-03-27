using MediatR;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Commands.RejectLeave;

public record RejectLeaveCommand(Guid LeaveId, RejectLeaveDto Dto) : IRequest<Result<LeaveDto>>;
