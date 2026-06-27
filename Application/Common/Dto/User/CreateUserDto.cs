namespace ProjectPlanner.Application.Common.Dto.User;

public record CreateUserDto(
    string Username,
    string Email,
    string Password
);