namespace ProjectPlanner.Application.Common.Dto.History;

public record CreateHistoryDto(
    int IssueId,
    int ChangedByUserId,
    string Field,
    string? OldValue,
    string? NewValue
);