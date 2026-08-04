namespace ProjectPlanner.Application.Common.Dtos.Project;

public record BacklogSprintDto(
    int Id,
    string Name,
    int IssueCount,
    DateTime? TargetStartDate);
