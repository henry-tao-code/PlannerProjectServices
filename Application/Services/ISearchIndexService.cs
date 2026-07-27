using Domain.Enums;

namespace ProjectPlanner.Application.Services;

public interface ISearchIndexService
{
    Task IndexIssueAsync(
        int issueId,
        CancellationToken ct = default);

    Task IndexEpicAsync(
        int epicId,
        CancellationToken ct = default);

    Task RemoveAsync(
        SearchEntityType entityType,
        int entityId,
        CancellationToken ct = default);
}