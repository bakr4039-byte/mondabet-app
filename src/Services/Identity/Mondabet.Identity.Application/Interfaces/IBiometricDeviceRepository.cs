using Mondabet.Identity.Domain.Entities;
using Mondabet.Shared.Application;

namespace Mondabet.Identity.Application.Interfaces;

public interface IBiometricDeviceRepository : IRepository<BiometricDevice>
{
    Task<BiometricDevice?> GetByDeviceIdAsync(string deviceId, CancellationToken ct = default);
    Task<IReadOnlyList<BiometricDevice>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}
