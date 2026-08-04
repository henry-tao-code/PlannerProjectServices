namespace ProjectPlanner.Application.Common.Dtos.Auth;

public record LoginRequestDto(
    string Username,
    string Password);
