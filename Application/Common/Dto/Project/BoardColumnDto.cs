using Domain.Enums;
using ProjectPlanner.Application.Common.Dto.Issue;

namespace ProjectPlanner.Application.Common.Dto.Project;

public record BoardColumnDto(
    IssueStatus StatusName,
    IEnumerable<BoardIssueDto> Issues);
