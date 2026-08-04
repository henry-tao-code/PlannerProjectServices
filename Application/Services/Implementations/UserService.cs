using Domain.Entities;
using ProjectPlanner.Application.Common.Dtos.Auth;
using ProjectPlanner.Application.Common.Dtos.User;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Application.Common.Interfaces.Security;

namespace ProjectPlanner.Application.Services.Implementations;

public class UserService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork) : IUserService
{
    // ---------------- REGISTER ----------------

    public async Task<UserResponseDto> RegisterAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        dto = dto with
        {
            Email = dto.Email.Trim().ToLowerInvariant(),
            Username = dto.Username.Trim()
        };

        if (await userRepository.ExistsByEmailAsync(dto.Email, ct))
            throw new InvalidOperationException("Email is already registered.");

        if (await userRepository.ExistsByUsernameAsync(dto.Username, ct))
            throw new InvalidOperationException("Username is already taken.");

        var user = User.CreateLocal(
            dto.Username,
            dto.Email,
            passwordHasher.HashPassword(dto.Password)
        );

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Map(user);
    }

    // ---------------- LOGIN ----------------

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
    {
        var user = await userRepository.GetByUsernameAsync(dto.Username, ct)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (user.IsDeleted ||
            string.IsNullOrWhiteSpace(user.PasswordHash) ||
            !passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var accessToken = jwtTokenGenerator.GenerateAccessToken(user.Id, user.Username);

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await refreshTokenRepository.AddAsync(refreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponseDto(
            user.Id,
            user.Username,
            accessToken,
            refreshToken.Token
        );
    }

    // ---------------- REFRESH TOKEN ----------------

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, CancellationToken ct = default)
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(dto.RefreshToken, ct)
            ?? throw new UnauthorizedAccessException("Invalid session token.");

        if (storedToken.IsRevoked || storedToken.IsExpired)
            throw new UnauthorizedAccessException("Session expired.");

        var user = await userRepository.GetByIdAsync(storedToken.UserId, ct)
            ?? throw new UnauthorizedAccessException("User not found.");

        storedToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        var accessToken = jwtTokenGenerator.GenerateAccessToken(user.Id, user.Username);

        await refreshTokenRepository.AddAsync(newRefreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponseDto(
            user.Id,
            user.Username,
            accessToken,
            newRefreshToken.Token
        );
    }

    // ---------------- PROFILE ----------------

    public async Task<UserResponseDto?> GetProfileByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(id, ct);
        return user is null ? null : Map(user);
    }

    public async Task<UserResponseDto?> GetProfileByUsernameAsync(string username, CancellationToken ct = default)
    {
        var user = await userRepository.GetByUsernameAsync(username, ct);
        return user is null ? null : Map(user);
    }

    public async Task<UserResponseDto> UpdateProfileAsync(
        int id,
        UpdateUserDto dto,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("User not found.");

        dto = dto with
        {
            Email = dto.Email.Trim().ToLowerInvariant()
        };

        if (user.Email != dto.Email &&
            await userRepository.ExistsByEmailAsync(dto.Email, ct))
        {
            throw new InvalidOperationException("Email already in use.");
        }

        user.Email = dto.Email;
        user.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(ct);

        return Map(user);
    }

    public async Task UpdatePasswordAsync(
        int id,
        UpdatePasswordDto dto,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("User not found.");

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
            throw new InvalidOperationException("Password login is not enabled for this user.");

        if (!passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.UpdatePassword(passwordHasher.HashPassword(dto.NewPassword));

        await unitOfWork.SaveChangesAsync(ct);
    }

    // ---------------- DELETE ----------------

    public async Task DeleteAccountAsync(int id, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(id, ct);

        if (user is null || user.IsDeleted)
            return;

        user.Delete();

        await unitOfWork.SaveChangesAsync(ct);
    }

    // ---------------- MAPPING ----------------

    private static UserResponseDto Map(User user)
        => new(user.Id, user.Username, user.Email);
}