namespace ProjectPlanner.Application.Common.Dtos.WorkLog;

public record CreateWorkLogDto(
    int IssueId,
    int UserId,
    int TimeSpentMinutes,
    string? Description,
    DateTime StartedAt,
    int? NewTimeRemainingMinutes = null);