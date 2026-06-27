using ProjectPlanner.Application.Common.Dto.History;

public interface IHistoryService
{
    Task<IEnumerable<HistoryDto>> GetByIssueIdAsync(
        int issueId,
        CancellationToken cancellationToken = default);

    Task LogChangeAsync(
        CreateHistoryDto dto,
        CancellationToken cancellationToken = default);
}