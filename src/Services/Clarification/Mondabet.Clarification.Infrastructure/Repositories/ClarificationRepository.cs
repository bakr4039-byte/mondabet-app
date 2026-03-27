using Microsoft.EntityFrameworkCore;
using Mondabet.Clarification.Application.Interfaces;
using Mondabet.Clarification.Infrastructure.Persistence;
using ClarificationEntity = Mondabet.Clarification.Domain.Entities.Clarification;

namespace Mondabet.Clarification.Infrastructure.Repositories;

public class ClarificationRepository : IClarificationRepository
{
    private readonly ClarificationDbContext _ctx;
    public ClarificationRepository(ClarificationDbContext ctx) => _ctx = ctx;

    public async Task<ClarificationEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Clarifications.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<ClarificationEntity>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Clarifications.ToListAsync(ct);

    public async Task<IReadOnlyList<ClarificationEntity>> GetByEmployeeAsync(
        Guid employeeId, CancellationToken ct = default)
        => await _ctx.Clarifications
            .Where(c => c.EmployeeId == employeeId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(ClarificationEntity entity, CancellationToken ct = default)
        => await _ctx.Clarifications.AddAsync(entity, ct);

    public void Update(ClarificationEntity entity) => _ctx.Clarifications.Update(entity);

    public void Delete(ClarificationEntity entity)
    {
        entity.IsDeleted = true;
        _ctx.Clarifications.Update(entity);
    }
}
