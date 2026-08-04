using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Epic;

public record EpicSummaryDto(
    int Id,
    string Name,
    string Summary,
    EpicStatus Status,
    string? AssigneeUsername,
    int TotalIssuesCount
);