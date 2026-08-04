namespace ProjectPlanner.Application.Common.Dtos.User;

public record GetUserDto(
    int Id,
    string Username,
    string Email,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
