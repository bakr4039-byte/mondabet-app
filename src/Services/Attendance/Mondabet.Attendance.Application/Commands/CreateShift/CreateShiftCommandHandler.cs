using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Attendance.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Commands.CreateShift;

public class CreateShiftCommandHandler : IRequestHandler<CreateShiftCommand, Result<ShiftDto>>
{
    private readonly IShiftRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateShiftCommandHandler(IShiftRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<ShiftDto>> Handle(CreateShiftCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var shift = Shift.Create(
            d.Name, d.StartTime, d.EndTime,
            d.Latitude, d.Longitude, d.RadiusMeters, d.DaysOfWeekJson);

        await _repo.AddAsync(shift, ct);
        await _uow.SaveChangesAsync(ct);
        return ToDto(shift);
    }

    internal static ShiftDto ToDto(Shift s) => new(
        s.Id, s.Name, s.StartTime, s.EndTime,
        s.Latitude, s.Longitude, s.RadiusMeters, s.DaysOfWeekJson);
}
