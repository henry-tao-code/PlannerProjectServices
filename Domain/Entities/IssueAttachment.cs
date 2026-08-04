using Domain.Enums;

namespace Domain.Entities;

public class IssueAttachment
{
    public long Id { get; set; }

    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public string OriginalFileName { get; set; } = null!;

    public string StoredFileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long FileSize { get; set; }

    public string StorageKey { get; set; } = null!;

    public string? Sha256Hash { get; set; }

    public int? UploadedByUserId { get; set; }
    public User? UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public AttachmentStatus Status { get; set; } = AttachmentStatus.Uploaded;
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessingError { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }
    public User? DeletedBy { get; set; }
}