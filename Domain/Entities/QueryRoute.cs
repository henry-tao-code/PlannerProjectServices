using Domain.Enums;

namespace Domain.Entities;

public class QueryRoute
{
    public SearchIntent Intent { get; init; }
    public bool UseKeywordSearch { get; init; }
    public bool UseVectorSearch { get; init; }
    public bool UseIssueSearch { get; init; }
    public bool UseDocumentSearch { get; init; }
    public int TopK { get; init; }
}