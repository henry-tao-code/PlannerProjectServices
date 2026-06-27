namespace ProjectPlanner.Application.Common.Dto.User;

public record GetUserDto(
    int Id,
    string Username,
    string Email,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint RowVersion
);
