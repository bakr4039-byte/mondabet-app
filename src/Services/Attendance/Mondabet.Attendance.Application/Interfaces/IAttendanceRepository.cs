using Mondabet.Attendance.Domain.Entities;
using Mondabet.Shared.Application;

namespace Mondabet.Attendance.Application.Interfaces;

public interface IAttendanceRepository : IRepository<AttendanceRecord>
{
    Task<bool> HasCheckInTodayAsync(Guid employeeId, Guid shiftId, CancellationToken ct = default);
    Task<AttendanceRecord?> GetOpenCheckInAsync(Guid employeeId, CancellationToken ct = default);
    Task<IReadOnlyList<AttendanceRecord>> GetByEmployeeAsync(
        Guid employeeId, DateTime from, DateTime to, CancellationToken ct = default);
    Task<(IReadOnlyList<AttendanceRecord> Items, int Total)> GetPagedAsync(
        Guid? employeeId, DateTime? from, DateTime? to,
        int page, int size, CancellationToken ct = default);
}
