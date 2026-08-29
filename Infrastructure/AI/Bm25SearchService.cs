using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectPlanner.Application.Common.Dtos.Search;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Infrastructure.Persistence;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProjectPlanner.Infrastructure.AI;

public partial class Bm25SearchService(
    IDbContextFactory<ProjectPlannerDbContext> contextFactory,
    ILogger<Bm25SearchService> logger) : IBm25SearchService
{
    private const double K1 = 1.5;
    private const double B = 0.75;

    public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        string query,
        int? projectId,
        int topK,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || topK <= 0)
        {
            return [];
        }

        var queryTerms = Tokenize(query).Distinct(StringComparer.Ordinal).ToArray();
        if (queryTerms.Length == 0)
        {
            return [];
        }

        var stopwatch = Stopwatch.StartNew();

        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

        var chunksQuery = dbContext.DocumentChunks
            .AsNoTracking()
            .Where(chunk => chunk.Content != null);

        if (projectId.HasValue)
        {
            chunksQuery = chunksQuery.Where(
                chunk => chunk.Attachment.Issue.ProjectId == projectId.Value);
        }

        var corpus = await chunksQuery
            .Select(chunk => new Bm25Document(
                chunk.Id,
                chunk.Attachment.Issue.ProjectId,
                chunk.Attachment.OriginalFileName,
                chunk.Content,
                chunk.ChunkType,
                chunk.PageNumber,
                chunk.TableIndex))
            .ToListAsync(cancellationToken);

        if (corpus.Count == 0)
        {
            return [];
        }

        var documents = corpus
            .Select(document => new TokenizedDocument(document, Tokenize(document.Content)))
            .Where(document => document.Terms.Length > 0)
            .ToList();

        if (documents.Count == 0)
        {
            return [];
        }

        var averageLength = documents.Average(document => document.Terms.Length);
        var documentFrequency = GetDocumentFrequency(documents, queryTerms);

        stopwatch.Stop();

        logger.LogWarning(
                "RAG retrieval through BM25 search in {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds);

        return [.. documents
            .Select(document => new
            {
                Document = document.Source,
                Score = Score(document, queryTerms, documentFrequency, documents.Count, averageLength)
            })
            .Where(result => result.Score > 0)
            .OrderByDescending(result => result.Score)
            .ThenBy(result => result.Document.Id)
            .Take(topK)
            .Select(result => new SearchResultDto(
                result.Document.Id,
                "Document",
                string.Empty,
                result.Document.Title,
                result.Document.Content,
                result.Document.ProjectId,
                null,
                null,
                null,
                null,
                result.Score)
            {
                ChunkType = result.Document.ChunkType,
                PageNumber = result.Document.PageNumber,
                TableIndex = result.Document.TableIndex
            })];
    }

    private static Dictionary<string, int> GetDocumentFrequency(
        IReadOnlyList<TokenizedDocument> documents,
        IReadOnlyCollection<string> queryTerms)
    {
        var frequencies = queryTerms.ToDictionary(term => term, _ => 0, StringComparer.Ordinal);

        foreach (var document in documents)
        {
            foreach (var term in document.TermFrequency.Keys)
            {
                if (frequencies.ContainsKey(term))
                {
                    frequencies[term]++;
                }
            }
        }

        return frequencies;
    }

    private static double Score(
        TokenizedDocument document,
        IReadOnlyCollection<string> queryTerms,
        IReadOnlyDictionary<string, int> documentFrequency,
        int corpusSize,
        double averageLength)
    {
        var score = 0.0;
        var lengthNormalization = K1 * (1 - B + B * document.Terms.Length / averageLength);

        foreach (var term in queryTerms)
        {
            if (!document.TermFrequency.TryGetValue(term, out var termFrequency))
            {
                continue;
            }

            var frequency = documentFrequency[term];
            var inverseDocumentFrequency = Math.Log(
                1 + (corpusSize - frequency + 0.5) / (frequency + 0.5));

            score += inverseDocumentFrequency *
                (termFrequency * (K1 + 1) / (termFrequency + lengthNormalization));
        }

        return score;
    }

    private static string[] Tokenize(string content) =>
        TokenRegex()
            .Matches(content.ToLowerInvariant())
            .Select(match => match.Value)
            .ToArray();

    [GeneratedRegex("[\\p{L}\\p{N}]+")]
    private static partial Regex TokenRegex();

    private sealed record Bm25Document(
        long Id,
        int ProjectId,
        string Title,
        string Content,
        string ChunkType,
        int? PageNumber,
        int? TableIndex);

    private sealed class TokenizedDocument
    {
        public TokenizedDocument(Bm25Document source, string[] terms)
        {
            Source = source;
            Terms = terms;
            TermFrequency = terms
                .GroupBy(term => term, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
        }

        public Bm25Document Source { get; }
        public string[] Terms { get; }
        public IReadOnlyDictionary<string, int> TermFrequency { get; }
    }
}
