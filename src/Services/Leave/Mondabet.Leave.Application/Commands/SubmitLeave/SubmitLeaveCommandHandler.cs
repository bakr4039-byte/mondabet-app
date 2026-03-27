using MediatR;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Leave.Application.Interfaces;
using Mondabet.Leave.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Commands.SubmitLeave;

public class SubmitLeaveCommandHandler : IRequestHandler<SubmitLeaveCommand, Result<LeaveDto>>
{
    private readonly ILeaveRepository _repo;
    private readonly IUnitOfWork _uow;

    public SubmitLeaveCommandHandler(ILeaveRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<LeaveDto>> Handle(SubmitLeaveCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var leave = LeaveRequest.Create(
            request.EmployeeId, d.LeaveType, d.StartDate, d.Reason,
            d.EndDate, d.StartTime, d.EndTime);

        await _repo.AddAsync(leave, ct);
        await _uow.SaveChangesAsync(ct);
        return ToDto(leave);
    }

    internal static LeaveDto ToDto(LeaveRequest l) => new(
        l.Id, l.EmployeeId, l.LeaveType,
        l.StartDate, l.EndDate, l.StartTime, l.EndTime,
        l.Reason, l.Status, l.RejectionReason, l.ApprovalComment,
        l.CreatedAt);
}
