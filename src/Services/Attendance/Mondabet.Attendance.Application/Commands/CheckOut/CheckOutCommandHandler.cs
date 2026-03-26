using MediatR;
using Mondabet.Attendance.Application.Commands.CheckIn;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Commands.CheckOut;

public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, Result<CheckInDto>>
{
    private readonly IAttendanceRepository _repo;
    private readonly IUnitOfWork _uow;

    public CheckOutCommandHandler(IAttendanceRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<CheckInDto>> Handle(CheckOutCommand request, CancellationToken ct)
    {
        var record = await _repo.GetOpenCheckInAsync(request.EmployeeId, ct);
        if (record is null)
            return Error.NotFound("AttendanceRecord", request.EmployeeId);

        record.CheckOut();
        _repo.Update(record);
        await _uow.SaveChangesAsync(ct);

        return CheckInCommandHandler.ToDto(record);
    }
}
