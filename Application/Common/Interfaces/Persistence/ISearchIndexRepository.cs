using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface ISearchIndexRepository
{
    Task EnsureIndexCreatedAsync(CancellationToken cancellationToken = default);
    Task IndexDocumentAsync(SearchDocument document, CancellationToken cancellationToken = default);
    Task BulkIndexDocumentsAsync(IEnumerable<SearchDocument> documents, CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync(string entityType, int entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SearchDocument>> SearchAsync(
        string query,
        int? projectId = null,
        string? status = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
