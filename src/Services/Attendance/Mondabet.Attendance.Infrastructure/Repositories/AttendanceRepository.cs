using Microsoft.EntityFrameworkCore;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Attendance.Domain.Entities;
using Mondabet.Attendance.Infrastructure.Persistence;

namespace Mondabet.Attendance.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AttendanceDbContext _ctx;
    public AttendanceRepository(AttendanceDbContext ctx) => _ctx = ctx;

    public async Task<AttendanceRecord?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.AttendanceRecords.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<AttendanceRecord>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.AttendanceRecords.ToListAsync(ct);

    public async Task AddAsync(AttendanceRecord entity, CancellationToken ct = default)
        => await _ctx.AttendanceRecords.AddAsync(entity, ct);

    public void Update(AttendanceRecord entity) => _ctx.AttendanceRecords.Update(entity);

    public void Delete(AttendanceRecord entity)
    {
        entity.IsDeleted = true;
        _ctx.AttendanceRecords.Update(entity);
    }

    public async Task<bool> HasCheckInTodayAsync(
        Guid employeeId, Guid shiftId, CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _ctx.AttendanceRecords.AnyAsync(
            a => a.EmployeeId == employeeId
              && a.ShiftId == shiftId
              && a.CheckInTime >= today
              && a.CheckInTime < today.AddDays(1),
            ct);
    }

    public async Task<AttendanceRecord?> GetOpenCheckInAsync(
        Guid employeeId, CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _ctx.AttendanceRecords.FirstOrDefaultAsync(
            a => a.EmployeeId == employeeId
              && a.CheckInTime >= today
              && a.CheckOutTime == null,
            ct);
    }

    public async Task<IReadOnlyList<AttendanceRecord>> GetByEmployeeAsync(
        Guid employeeId, DateTime from, DateTime to, CancellationToken ct = default)
        => await _ctx.AttendanceRecords
            .Where(a => a.EmployeeId == employeeId
                     && a.CheckInTime >= from
                     && a.CheckInTime <= to)
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync(ct);

    public async Task<(IReadOnlyList<AttendanceRecord> Items, int Total)> GetPagedAsync(
        Guid? employeeId, DateTime? from, DateTime? to,
        int page, int size, CancellationToken ct = default)
    {
        var query = _ctx.AttendanceRecords.AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(a => a.EmployeeId == employeeId.Value);
        if (from.HasValue)
            query = query.Where(a => a.CheckInTime >= from.Value);
        if (to.HasValue)
            query = query.Where(a => a.CheckInTime <= to.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.CheckInTime)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        return (items, total);
    }
}
