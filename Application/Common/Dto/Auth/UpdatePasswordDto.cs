public record UpdatePasswordDto(
    string CurrentPassword,
    string NewPassword,
    uint RowVersion);