using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(ProjectPlannerDbContext dbContext) : IRefreshTokenRepository
{
    private readonly ProjectPlannerDbContext _dbContext = dbContext;

    // ---------------- READ ----------------

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        return await _dbContext.Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Token == token, ct);
    }

    // ---------------- CREATE ----------------

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
    {
        await _dbContext.Set<RefreshToken>().AddAsync(token, ct);
    }

    // ---------------- REVOKE SINGLE ----------------

    public async Task RevokeAsync(RefreshToken token, CancellationToken ct = default)
    {
        var existing = await _dbContext.Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Id == token.Id, ct);

        if (existing is null)
            return;

        existing.RevokedAt = DateTime.UtcNow;
    }

    // ---------------- REVOKE ALL USER TOKENS ----------------

    public async Task RevokeAllForUserAsync(int userId, CancellationToken ct = default)
    {
        var tokens = await _dbContext.Set<RefreshToken>()
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }
    }
}