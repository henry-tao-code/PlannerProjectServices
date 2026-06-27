namespace ProjectPlanner.Application.Common.Dto.User;

public record UserResponseDto(
    int Id,
    string Username,
    string Email,
    uint RowVersion);
