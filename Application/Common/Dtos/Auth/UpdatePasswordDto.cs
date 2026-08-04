namespace ProjectPlanner.Application.Common.Dtos.Auth;

public record UpdatePasswordDto(
    string CurrentPassword,
    string NewPassword);