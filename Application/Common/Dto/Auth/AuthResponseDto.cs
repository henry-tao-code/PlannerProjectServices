namespace ProjectPlanner.Application.Common.Dto.Auth;

public record AuthResponseDto(
    int Id,
    string Username,
    string AccessToken,
    string RefreshToken);