namespace ProjectPlanner.Application.Common.Dtos.Issue;

public class IssueAttachmentDto
{
    public long Id { get; set; }

    public int IssueId { get; set; }

    public string OriginalFileName { get; set; } = null!;

    public string StoredFileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; }
}
