using MediatR;
using Mondabet.Leave.Application.Commands.SubmitLeave;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Leave.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Queries.GetLeave;

public class GetLeaveQueryHandler : IRequestHandler<GetLeaveQuery, Result<LeaveDto>>
{
    private readonly ILeaveRepository _repo;
    public GetLeaveQueryHandler(ILeaveRepository repo) => _repo = repo;

    public async Task<Result<LeaveDto>> Handle(GetLeaveQuery request, CancellationToken ct)
    {
        var leave = await _repo.GetByIdAsync(request.LeaveId, ct);
        if (leave is null) return Error.NotFound("LeaveRequest", request.LeaveId);
        return SubmitLeaveCommandHandler.ToDto(leave);
    }
}
