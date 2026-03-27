using MediatR;
using Mondabet.Leave.Application.Commands.SubmitLeave;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Leave.Application.Interfaces;
using Mondabet.Leave.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Commands.ApproveLeave;

public class ApproveLeaveCommandHandler : IRequestHandler<ApproveLeaveCommand, Result<LeaveDto>>
{
    private readonly ILeaveRepository _repo;
    private readonly IUnitOfWork _uow;

    public ApproveLeaveCommandHandler(ILeaveRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<LeaveDto>> Handle(ApproveLeaveCommand request, CancellationToken ct)
    {
        var leave = await _repo.GetByIdAsync(request.LeaveId, ct);
        if (leave is null) return Error.NotFound("LeaveRequest", request.LeaveId);
        if (leave.Status != LeaveStatus.Pending)
            return Error.Conflict("Leave request is no longer pending.");

        leave.Approve(request.Dto.Comment);
        _repo.Update(leave);
        await _uow.SaveChangesAsync(ct);
        return SubmitLeaveCommandHandler.ToDto(leave);
    }
}
