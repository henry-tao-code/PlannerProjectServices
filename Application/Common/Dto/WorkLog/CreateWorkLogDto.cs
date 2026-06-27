namespace ProjectPlanner.Application.Common.Dto.WorkLog;

public record CreateWorkLogDto(
    int IssueId,
    int UserId,
    int TimeSpentMinutes,
    string? Description,
    DateTime StartedAt,
    int? NewTimeRemainingMinutes = null);