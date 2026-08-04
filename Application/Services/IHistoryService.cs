using ProjectPlanner.Application.Common.Dtos.History;

namespace ProjectPlanner.Application.Services;

public interface IHistoryService
{
    Task<IEnumerable<HistoryDto>> GetByIssueIdAsync(
        int issueId,
        CancellationToken cancellationToken = default);

    Task LogChangeAsync(
        CreateHistoryDto dto,
        CancellationToken cancellationToken = default);
}