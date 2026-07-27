using Domain.Enums;

namespace Domain.Entities;

public class SearchDocument
{
    public int Id { get; set; }
    public SearchEntityType EntityType { get; set; }
    public int EntityId { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
}