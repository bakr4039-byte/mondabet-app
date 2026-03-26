using Microsoft.EntityFrameworkCore;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Domain.Entities;
using Mondabet.Identity.Infrastructure.Persistence;

namespace Mondabet.Identity.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _context;

    public RefreshTokenRepository(IdentityDbContext context) => _context = context;

    public async Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<RefreshToken>> GetAllAsync(CancellationToken ct = default)
        => await _context.RefreshTokens.ToListAsync(ct);

    public async Task AddAsync(RefreshToken entity, CancellationToken ct = default)
        => await _context.RefreshTokens.AddAsync(entity, ct);

    public void Update(RefreshToken entity) => _context.RefreshTokens.Update(entity);

    public void Delete(RefreshToken entity)
    {
        entity.IsDeleted = true;
        _context.RefreshTokens.Update(entity);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => await _context.RefreshTokens.IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Token == token, ct);

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(r => r.UserId == userId)
            .ToListAsync(ct);

        foreach (var token in tokens)
            token.Revoke();
    }
}
