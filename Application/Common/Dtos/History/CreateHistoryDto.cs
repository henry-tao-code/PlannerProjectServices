namespace ProjectPlanner.Application.Common.Dtos.History;

public record CreateHistoryDto(
    int IssueId,
    int ChangedByUserId,
    string Field,
    string? OldValue,
    string? NewValue
);