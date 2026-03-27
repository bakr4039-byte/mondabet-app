using MediatR;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Queries.GetLeave;

public record GetLeaveQuery(Guid LeaveId) : IRequest<Result<LeaveDto>>;
