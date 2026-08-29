using Domain.Entities;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class SearchIndexRepository(
    ElasticsearchClient client
    , ILogger<SearchIndexRepository> logger
    ) : ISearchIndexRepository
{
    public const string IndexName = "search-documents";

    private static readonly string[] SearchFields = ["title^2", "description", "issueKey"];

    public async Task EnsureIndexCreatedAsync(CancellationToken cancellationToken = default)
    {
        var existsResponse = await client.Indices.ExistsAsync(IndexName, cancellationToken);

        if (!existsResponse.Exists)
        {
            logger.LogInformation("Elasticsearch index '{IndexName}' does not exist. Creating now...", IndexName);

            var createResponse = await client.Indices.CreateAsync<SearchDocument>(IndexName, c => c
                .Mappings(m => m
                    .Properties(p => p
                        .IntegerNumber(f => f.EntityId)
                        .Keyword(f => f.EntityType)
                        .Keyword(f => f.IssueKey)
                        .Text(f => f.Title)
                        .Text(f => f.Description)
                        .IntegerNumber(f => f.ProjectId)
                        .Text(f => f.ProjectName)
                        .Keyword(f => f.Status)
                        .Keyword(f => f.Priority)
                        .Text(f => f.AssigneeName)
                        .Text(f => f.ReporterName)
                        .IntegerNumber(f => f.SprintId)
                        .Keyword(f => f.SprintName)
                    )
                ),
                cancellationToken);

            if (!createResponse.IsValidResponse)
            {
                logger.LogError("Failed to create index '{IndexName}': {Error}", IndexName, createResponse.DebugInformation);
                throw new InvalidOperationException($"Failed to create Elasticsearch index '{IndexName}': {createResponse.DebugInformation}");
            }

            logger.LogInformation("Elasticsearch index '{IndexName}' successfully created.", IndexName);
        }
    }

    public async Task IndexDocumentAsync(SearchDocument document, CancellationToken cancellationToken = default)
    {
        var documentId = FormatDocumentId(document.EntityType, document.EntityId);

        var response = await client.IndexAsync(document, idx => idx
            .Index(IndexName)
            .Id(documentId),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Failed to index document ID '{DocumentId}': {Error}", documentId, response.DebugInformation);
            throw new InvalidOperationException($"Error indexing document {documentId}: {response.DebugInformation}");
        }
    }

    public async Task BulkIndexDocumentsAsync(IEnumerable<SearchDocument> documents, CancellationToken cancellationToken = default)
    {
        var documentList = documents.ToList();
        if (documentList.Count == 0) return;

        var response = await client.BulkAsync(b => b
            .Index(IndexName)
            .IndexMany(documentList, (descriptor, doc) => descriptor.Id(FormatDocumentId(doc.EntityType, doc.EntityId))),
            cancellationToken);

        if (response.Errors)
        {
            logger.LogError("Errors occurred during bulk indexing: {Error}", response.DebugInformation);
            throw new InvalidOperationException($"Bulk indexing completed with errors: {response.DebugInformation}");
        }
    }

    public async Task DeleteDocumentAsync(string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        var documentId = FormatDocumentId(entityType, entityId);

        var response = await client.DeleteAsync<SearchDocument>(documentId, d => d
            .Index(IndexName),
            cancellationToken);

        if (!response.IsValidResponse && response.Result != Result.NotFound)
        {
            logger.LogError("Failed to delete document '{DocumentId}': {Error}", documentId, response.DebugInformation);
            throw new InvalidOperationException($"Error deleting document {documentId}: {response.DebugInformation}");
        }
    }

    public async Task<IEnumerable<SearchDocument>> SearchAsync(
        string query,
        int? projectId = null,
        string? status = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var from = (page - 1) * pageSize;

        var response = await client.SearchAsync<SearchDocument>(s => s
            .Indices(IndexName)
            .From(from)
            .Size(pageSize)
            .Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .MultiMatch(mm => mm
                            .Query(query)
                            .Fields(SearchFields)
                            .Fuzziness(new Fuzziness("AUTO"))
                        )
                    )
                    .Filter(f =>
                    {
                        if (projectId.HasValue)
                        {
                            f.Term(t => t.Field(ff => ff.ProjectId).Value(projectId.Value));
                        }
                        if (!string.IsNullOrWhiteSpace(status))
                        {
                            f.Term(t => t.Field(ff => ff.Status).Value(status));
                        }
                    })
                )
            ),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Elasticsearch search failed: {Error}", response.DebugInformation);
            return [];
        }

        return response.Documents;
    }

    private static string FormatDocumentId(string entityType, int entityId) => $"{entityType.ToLowerInvariant()}_{entityId}";
}