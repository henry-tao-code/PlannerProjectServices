namespace ProjectPlanner.Application.Common.Dto.Project;

public record BacklogSprintDto(
    int Id,
    string Name,
    int IssueCount,
    DateTime? TargetStartDate);
