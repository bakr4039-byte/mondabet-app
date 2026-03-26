using Microsoft.EntityFrameworkCore;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Domain.Entities;
using Mondabet.Identity.Infrastructure.Persistence;

namespace Mondabet.Identity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
        => await _context.Users.ToListAsync(ct);

    public async Task AddAsync(User entity, CancellationToken ct = default)
        => await _context.Users.AddAsync(entity, ct);

    public void Update(User entity) => _context.Users.Update(entity);

    public void Delete(User entity)
    {
        entity.IsDeleted = true;
        _context.Users.Update(entity);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(
            u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<User?> GetByMobileAsync(string mobile, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.MobileNumber == mobile, ct);
}
