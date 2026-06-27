namespace ProjectPlanner.Application.Common.Dto.Issue;

public record UpdateIssueLabelsDto(
    List<string> Labels,
    uint RowVersion
);