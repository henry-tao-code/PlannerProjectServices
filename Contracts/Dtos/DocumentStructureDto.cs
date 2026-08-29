using System.Text.Json;

namespace Contracts.Dtos;

public record DocumentStructureDto
{
    public DocumentStructureMetadataDto Metadata { get; init; } = new();
    public IReadOnlyList<DocumentPageDto> Pages { get; init; } = [];
    public IReadOnlyList<DocumentTableDto> Tables { get; init; } = [];
    public IReadOnlyList<DocumentPictureDto> Pictures { get; init; } = [];
}

public record DocumentStructureMetadataDto
{
    public string DocumentName { get; init; } = string.Empty;
    public int PageCount { get; init; }
    public int TextBlockCount { get; init; }
    public int TableCount { get; init; }
    public int PictureCount { get; init; }
}

public record DocumentPageDto
{
    public int Page { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
}

public record DocumentTableDto
{
    public int Index { get; init; }
    public int? Page { get; init; }
    public IReadOnlyList<double> Bbox { get; init; } = [];
    public int RowCount { get; init; }
    public int ColumnCount { get; init; }
    public string Markdown { get; init; } = string.Empty;
    public string Html { get; init; } = string.Empty;
    public JsonElement Data { get; init; }
}

public record DocumentPictureDto
{
    public int Index { get; init; }
    public int? Page { get; init; }
    public IReadOnlyList<double> Bbox { get; init; } = [];
}
