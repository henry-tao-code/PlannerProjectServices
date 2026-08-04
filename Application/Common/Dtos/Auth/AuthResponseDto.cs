namespace ProjectPlanner.Application.Common.Dtos.Auth;

public record AuthResponseDto(
    int Id,
    string Username,
    string AccessToken,
    string RefreshToken);