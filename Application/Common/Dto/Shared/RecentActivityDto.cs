namespace ProjectPlanner.Application.Common.Dto.Shared;

public record RecentActivityDto(
    int Id,
    string Message,
    string PerformedBy,
    DateTime Timestamp);