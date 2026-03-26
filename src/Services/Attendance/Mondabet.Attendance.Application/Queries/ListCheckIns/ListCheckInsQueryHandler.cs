using MediatR;
using Mondabet.Attendance.Application.Commands.CheckIn;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Queries.ListCheckIns;

public class ListCheckInsQueryHandler
    : IRequestHandler<ListCheckInsQuery, Result<PagedResult<CheckInDto>>>
{
    private readonly IAttendanceRepository _repo;

    public ListCheckInsQueryHandler(IAttendanceRepository repo) => _repo = repo;

    public async Task<Result<PagedResult<CheckInDto>>> Handle(
        ListCheckInsQuery request, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            request.EmployeeId, request.From, request.To,
            request.Page, request.Size, ct);

        var dtos = items.Select(CheckInCommandHandler.ToDto).ToList();
        return new PagedResult<CheckInDto>(dtos, total, request.Page, request.Size);
    }
}
