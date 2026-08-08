using Domain.Entities;
using Domain.Enums;
using System.Text.RegularExpressions;

namespace ProjectPlanner.Application.Services.Implementations;

public class QueryRouterService : IQueryRouterService
{
    private static readonly Regex IssueKeyRegex =
        new(@"[A-Z][A-Z0-9]+-\d+",
            RegexOptions.Compiled);

    public QueryRoute Route(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException(
                "Query cannot be empty.",
                nameof(query));
        }

        if (IssueKeyRegex.IsMatch(query))
        {
            return new QueryRoute
            {
                Intent = SearchIntent.IssueLookup,
                UseKeywordSearch = true,
                UseVectorSearch = false,
                UseIssueSearch = true,
                UseDocumentSearch = false,
                TopK = 5
            };
        }

        if (LooksLikeKnowledgeQuestion(query))
        {
            return new QueryRoute
            {
                Intent = SearchIntent.KnowledgeQuestion,
                UseKeywordSearch = true,
                UseVectorSearch = true,
                UseIssueSearch = true,
                UseDocumentSearch = true,
                TopK = 10
            };
        }

        return new QueryRoute
        {
            Intent = SearchIntent.IssueSearch,
            UseKeywordSearch = true,
            UseVectorSearch = true,
            UseIssueSearch = true,
            UseDocumentSearch = false,
            TopK = 10
        };
    }

    private static bool LooksLikeKnowledgeQuestion(string query)
    {
        var keywords = new[]
        {
            "how",
            "why",
            "what",
            "explain",
            "documentation",
            "guide",
            "architecture"
        };

        return keywords.Any(k =>
            query.Contains(k, StringComparison.OrdinalIgnoreCase));
    }
}