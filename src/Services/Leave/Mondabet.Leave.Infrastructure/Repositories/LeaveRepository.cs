using Microsoft.EntityFrameworkCore;
using Mondabet.Leave.Application.Interfaces;
using Mondabet.Leave.Domain.Entities;
using Mondabet.Leave.Infrastructure.Persistence;

namespace Mondabet.Leave.Infrastructure.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly LeaveDbContext _ctx;
    public LeaveRepository(LeaveDbContext ctx) => _ctx = ctx;

    public async Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<IReadOnlyList<LeaveRequest>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.LeaveRequests.ToListAsync(ct);

    public async Task AddAsync(LeaveRequest entity, CancellationToken ct = default)
        => await _ctx.LeaveRequests.AddAsync(entity, ct);

    public void Update(LeaveRequest entity) => _ctx.LeaveRequests.Update(entity);

    public void Delete(LeaveRequest entity)
    {
        entity.IsDeleted = true;
        _ctx.LeaveRequests.Update(entity);
    }

    public async Task<(IReadOnlyList<LeaveRequest> Items, int Total)> GetPagedAsync(
        Guid? employeeId, LeaveType? type, LeaveStatus? status,
        int page, int size, CancellationToken ct = default)
    {
        var query = _ctx.LeaveRequests.AsQueryable();

        if (employeeId.HasValue) query = query.Where(l => l.EmployeeId == employeeId.Value);
        if (type.HasValue) query = query.Where(l => l.LeaveType == type.Value);
        if (status.HasValue) query = query.Where(l => l.Status == status.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        return (items, total);
    }
}
