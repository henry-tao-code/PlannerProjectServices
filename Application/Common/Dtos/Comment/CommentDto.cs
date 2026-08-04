using ProjectPlanner.Application.Common.Dtos.Attachment;

namespace ProjectPlanner.Application.Common.Dto.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public string Body { get; set; } = null!;
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsEdited => EditedAt.HasValue;
    public List<AttachmentDto> Attachments { get; set; } = [];
}