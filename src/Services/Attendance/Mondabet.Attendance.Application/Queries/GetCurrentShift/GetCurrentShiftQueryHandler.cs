using MediatR;
using Mondabet.Attendance.Application.Commands.CreateShift;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Queries.GetCurrentShift;

public class GetCurrentShiftQueryHandler : IRequestHandler<GetCurrentShiftQuery, Result<ShiftDto>>
{
    private readonly IEmployeeLookupService _employeeLookup;
    private readonly IShiftRepository _shiftRepo;

    public GetCurrentShiftQueryHandler(IEmployeeLookupService employeeLookup, IShiftRepository shiftRepo)
    {
        _employeeLookup = employeeLookup;
        _shiftRepo = shiftRepo;
    }

    public async Task<Result<ShiftDto>> Handle(GetCurrentShiftQuery request, CancellationToken ct)
    {
        var shiftId = await _employeeLookup.GetShiftIdAsync(request.EmployeeId, ct);
        if (shiftId is null)
            return Error.NotFound("Shift", request.EmployeeId);

        var shift = await _shiftRepo.GetByIdAsync(shiftId.Value, ct);
        if (shift is null)
            return Error.NotFound("Shift", shiftId.Value);

        return CreateShiftCommandHandler.ToDto(shift);
    }
}
