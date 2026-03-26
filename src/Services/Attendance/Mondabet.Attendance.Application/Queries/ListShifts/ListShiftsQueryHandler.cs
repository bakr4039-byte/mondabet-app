using MediatR;
using Mondabet.Attendance.Application.Commands.CreateShift;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Queries.ListShifts;

public class ListShiftsQueryHandler : IRequestHandler<ListShiftsQuery, Result<IReadOnlyList<ShiftDto>>>
{
    private readonly IShiftRepository _repo;

    public ListShiftsQueryHandler(IShiftRepository repo) => _repo = repo;

    public async Task<Result<IReadOnlyList<ShiftDto>>> Handle(
        ListShiftsQuery request, CancellationToken ct)
    {
        var shifts = await _repo.GetAllAsync(ct);
        return shifts.Select(CreateShiftCommandHandler.ToDto).ToList();
    }
}
