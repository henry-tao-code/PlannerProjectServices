namespace ProjectPlanner.Application.Common.Dto.Project;

public record ProjectCalendarEventDto(
    int IssueId,
    string IssueKey,
    string Title,
    DateTime StartDate,
    DateTime DueDate,
    string ColorHex);
