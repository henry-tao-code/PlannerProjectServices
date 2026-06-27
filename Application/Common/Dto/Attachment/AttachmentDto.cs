namespace ProjectPlanner.Application.Common.Dto.Attachment;

public class AttachmentDto
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }

    public DateTime UploadedAt { get; set; }

    public int UploadedByUserId { get; set; }
    public string? UploadedByName { get; set; }

    public string DownloadUrl { get; set; } = null!;

    public bool IsImage { get; set; }
    public string? ThumbnailUrl { get; set; }
}