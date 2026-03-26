using Microsoft.EntityFrameworkCore;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Domain.Entities;
using Mondabet.Identity.Infrastructure.Persistence;

namespace Mondabet.Identity.Infrastructure.Repositories;

public class BiometricDeviceRepository : IBiometricDeviceRepository
{
    private readonly IdentityDbContext _context;

    public BiometricDeviceRepository(IdentityDbContext context) => _context = context;

    public async Task<BiometricDevice?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.BiometricDevices.FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<IReadOnlyList<BiometricDevice>> GetAllAsync(CancellationToken ct = default)
        => await _context.BiometricDevices.ToListAsync(ct);

    public async Task AddAsync(BiometricDevice entity, CancellationToken ct = default)
        => await _context.BiometricDevices.AddAsync(entity, ct);

    public void Update(BiometricDevice entity) => _context.BiometricDevices.Update(entity);

    public void Delete(BiometricDevice entity)
    {
        entity.IsDeleted = true;
        _context.BiometricDevices.Update(entity);
    }

    public async Task<BiometricDevice?> GetByDeviceIdAsync(string deviceId, CancellationToken ct = default)
        => await _context.BiometricDevices.FirstOrDefaultAsync(b => b.DeviceId == deviceId, ct);

    public async Task<IReadOnlyList<BiometricDevice>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _context.BiometricDevices.Where(b => b.UserId == userId).ToListAsync(ct);
}
