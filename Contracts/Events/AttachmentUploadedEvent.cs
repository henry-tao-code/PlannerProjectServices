namespace Contracts.Events;

public record AttachmentUploadedEvent
(
    long AttachmentId,
    int IssueId,
    string StorageKey,
    string OriginalFileName,
    string ContentType,
    long FileSize,
    DateTime UploadedAt
);