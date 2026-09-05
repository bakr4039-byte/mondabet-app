using MediatR;
using Mondabet.Attendance.Application.Commands.CreateShift;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Commands.UpdateShift;

public class UpdateShiftCommandHandler : IRequestHandler<UpdateShiftCommand, Result<ShiftDto>>
{
    private readonly IShiftRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateShiftCommandHandler(IShiftRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<ShiftDto>> Handle(UpdateShiftCommand request, CancellationToken ct)
    {
        var shift = await _repo.GetByIdAsync(request.ShiftId, ct);
        if (shift is null) return Error.NotFound("Shift", request.ShiftId);

        var d = request.Dto;
        shift.Update(d.Name, d.StartTime, d.EndTime,
            d.Latitude, d.Longitude, d.RadiusMeters, d.DaysOfWeekJson,
            d.GracePeriodMinutes, d.WindowStartMinutes, d.WindowEndMinutes,
            d.IsSplitShift, d.FirstStartTime, d.FirstEndTime, d.SecondStartTime, d.SecondEndTime);

        _repo.Update(shift);
        await _uow.SaveChangesAsync(ct);
        return CreateShiftCommandHandler.ToDto(shift);
    }
}
