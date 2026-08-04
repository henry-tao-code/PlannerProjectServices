namespace ProjectPlanner.Application.Common.Dtos.Shared;

public record TimelineItemDto(
    int Id,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    string Status);