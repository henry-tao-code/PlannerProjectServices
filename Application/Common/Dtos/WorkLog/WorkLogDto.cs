namespace ProjectPlanner.Application.Common.Dtos.WorkLog;

public record WorkLogDto(
    int Id,
    int IssueId,
    int UserId,
    string Username,
    int TimeSpentMinutes,
    string? Description,
    DateTime StartedAt,
    DateTime CreatedAt);
