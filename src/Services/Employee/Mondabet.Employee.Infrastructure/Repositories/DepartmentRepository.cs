using Microsoft.EntityFrameworkCore;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Employee.Domain.Entities;
using Mondabet.Employee.Infrastructure.Persistence;

namespace Mondabet.Employee.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly EmployeeDbContext _ctx;
    public DepartmentRepository(EmployeeDbContext ctx) => _ctx = ctx;

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Departments.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Departments.OrderBy(d => d.Name).ToListAsync(ct);

    public async Task AddAsync(Department entity, CancellationToken ct = default)
        => await _ctx.Departments.AddAsync(entity, ct);

    public void Update(Department entity) => _ctx.Departments.Update(entity);

    public void Delete(Department entity)
    {
        entity.IsDeleted = true;
        _ctx.Departments.Update(entity);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken ct = default)
        => await _ctx.Departments.AnyAsync(
            d => d.Code == code && (excludeId == null || d.Id != excludeId), ct);
}
