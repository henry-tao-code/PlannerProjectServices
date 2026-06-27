namespace ProjectPlanner.Application.Common.Dto.History;

public record HistoryDto(
    int Id,
    int IssueId,
    string Field,
    string? OldValue,
    string? NewValue,
    int ChangedByUserId,
    DateTime ChangedAt
);