using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);

    Task AddAsync(RefreshToken token, CancellationToken ct = default);

    Task RevokeAsync(RefreshToken token, CancellationToken ct = default);

    Task RevokeAllForUserAsync(int userId, CancellationToken ct = default);
}