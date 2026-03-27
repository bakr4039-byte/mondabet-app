using MediatR;
using Mondabet.Leave.Application.Commands.SubmitLeave;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Leave.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Queries.ListLeaves;

public class ListLeavesQueryHandler
    : IRequestHandler<ListLeavesQuery, Result<PagedResult<LeaveDto>>>
{
    private readonly ILeaveRepository _repo;
    public ListLeavesQueryHandler(ILeaveRepository repo) => _repo = repo;

    public async Task<Result<PagedResult<LeaveDto>>> Handle(
        ListLeavesQuery request, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            request.EmployeeId, request.Type, request.Status,
            request.Page, request.Size, ct);

        var dtos = items.Select(SubmitLeaveCommandHandler.ToDto).ToList();
        return new PagedResult<LeaveDto>(dtos, total, request.Page, request.Size);
    }
}
