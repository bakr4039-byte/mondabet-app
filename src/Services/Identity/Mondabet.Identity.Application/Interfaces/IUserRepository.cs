using Mondabet.Identity.Domain.Entities;
using Mondabet.Shared.Application;

namespace Mondabet.Identity.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByMobileAsync(string mobile, CancellationToken ct = default);
}
