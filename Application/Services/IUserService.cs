using ProjectPlanner.Application.Common.Dto.Auth;
using ProjectPlanner.Application.Common.Dto.User;

namespace ProjectPlanner.Application.Services;

public interface IUserService
{
    // Authentication 
    Task<UserResponseDto> RegisterAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, CancellationToken cancellationToken = default);

    // Profile Management 
    Task<UserResponseDto?> GetProfileByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserResponseDto?> GetProfileByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<UserResponseDto> UpdateProfileAsync(int id, UpdateUserDto dto, uint clientRowVersion, CancellationToken cancellationToken = default);
    Task UpdatePasswordAsync(int id, UpdatePasswordDto dto, CancellationToken cancellationToken = default);
    Task DeleteAccountAsync(int id, CancellationToken cancellationToken = default);
}