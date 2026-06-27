namespace ProjectPlanner.Application.Common.Dto.Shared;

public record TimelineItemDto(
    int Id,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    string Status);