using Domain.Enums;
using ProjectPlanner.Application.Common.Dtos.Issue;

namespace ProjectPlanner.Application.Common.Dtos.Project;

public record BoardColumnDto(
    IssueStatus StatusName,
    IEnumerable<BoardIssueDto> Issues);
