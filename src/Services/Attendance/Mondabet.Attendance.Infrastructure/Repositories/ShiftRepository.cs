using Microsoft.EntityFrameworkCore;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Attendance.Domain.Entities;
using Mondabet.Attendance.Infrastructure.Persistence;

namespace Mondabet.Attendance.Infrastructure.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly AttendanceDbContext _ctx;
    public ShiftRepository(AttendanceDbContext ctx) => _ctx = ctx;

    public async Task<Shift?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Shifts.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Shift>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Shifts.ToListAsync(ct);

    public async Task AddAsync(Shift entity, CancellationToken ct = default)
        => await _ctx.Shifts.AddAsync(entity, ct);

    public void Update(Shift entity) => _ctx.Shifts.Update(entity);

    public void Delete(Shift entity)
    {
        entity.IsDeleted = true;
        _ctx.Shifts.Update(entity);
    }
}
