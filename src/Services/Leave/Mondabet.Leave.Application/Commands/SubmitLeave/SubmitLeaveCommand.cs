using MediatR;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Commands.SubmitLeave;

public record SubmitLeaveCommand(Guid EmployeeId, SubmitLeaveDto Dto) : IRequest<Result<LeaveDto>>;
