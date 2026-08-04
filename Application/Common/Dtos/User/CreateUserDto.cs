namespace ProjectPlanner.Application.Common.Dtos.User;

public record CreateUserDto(
    string Username,
    string Email,
    string Password
);