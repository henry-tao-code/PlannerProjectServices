using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class UserRepository(ProjectPlannerDbContext dbContext) : IUserRepository
{
    private readonly ProjectPlannerDbContext _dbContext = dbContext;

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<User?> GetByAuthProviderIdAsync(string provider, string providerId, CancellationToken ct = default)
        => await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.AuthProvider == provider &&
            u.AuthProviderId == providerId,
            ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await _dbContext.Users.AnyAsync(u => u.Email == email, ct);

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
        => await _dbContext.Users.AnyAsync(u => u.Username == username, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _dbContext.Users.AddAsync(user, ct);

    public void Update(User user)
        => _dbContext.Users.Update(user);

    public void Delete(User user)
    {
        user.Delete();
    }
}