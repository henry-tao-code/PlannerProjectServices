namespace ProjectPlanner.Application.Common.Dtos.Shared;

public record RecentActivityDto(
    int Id,
    string Message,
    string PerformedBy,
    DateTime Timestamp);