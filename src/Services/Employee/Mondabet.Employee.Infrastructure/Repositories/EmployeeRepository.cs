using Microsoft.EntityFrameworkCore;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Employee.Infrastructure.Persistence;

namespace Mondabet.Employee.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeDbContext _ctx;
    public EmployeeRepository(EmployeeDbContext ctx) => _ctx = ctx;

    public async Task<Domain.Entities.Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Domain.Entities.Employee>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Employees.ToListAsync(ct);

    public async Task AddAsync(Domain.Entities.Employee entity, CancellationToken ct = default)
        => await _ctx.Employees.AddAsync(entity, ct);

    public void Update(Domain.Entities.Employee entity) => _ctx.Employees.Update(entity);

    public void Delete(Domain.Entities.Employee entity)
    {
        entity.IsDeleted = true;
        _ctx.Employees.Update(entity);
    }

    public async Task<Domain.Entities.Employee?> GetByIqamaAsync(string iqama, CancellationToken ct = default)
        => await _ctx.Employees.FirstOrDefaultAsync(e => e.Iqama == iqama, ct);

    public async Task<bool> IqamaExistsAsync(string iqama, CancellationToken ct = default)
        => await _ctx.Employees.AnyAsync(e => e.Iqama == iqama, ct);

    public async Task<(IReadOnlyList<Domain.Entities.Employee> Items, int Total)> GetPagedAsync(
        int page, int size, string? search, CancellationToken ct = default)
    {
        var query = _ctx.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e =>
                e.FullNameEn.Contains(search) ||
                e.FullNameAr.Contains(search) ||
                e.Iqama.Contains(search) ||
                e.Email.Contains(search));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(e => e.FullNameEn)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<int> CountByDepartmentAsync(Guid departmentId, CancellationToken ct = default)
        => await _ctx.Employees.CountAsync(e => e.DepartmentId == departmentId, ct);
}
