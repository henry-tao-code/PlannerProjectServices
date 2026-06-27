using Domain.Entities;

public class IssueAttachment
{
    public long Id { get; set; }

    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public string FileName { get; set; } = null!;
    public string StoredFileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }

    public string FilePath { get; set; } = null!;
    public int UploadedByUserId { get; set; }
    public User UploadedBy { get; set; } = null!;

    public DateTime UploadedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }
    public uint RowVersion { get; set; }
}