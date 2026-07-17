namespace Domain.Entities;

public class Attachment
{
    public int Id { get; private set; }
    public string FileName { get; private set; } = null!;
    public string StoragePath { get; private set; } = null!; // URL or local relative path
    public string ContentType { get; private set; } = null!;
    public long FileSize { get; private set; }

    public int IssueId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Attachment Create(string fileName, string storagePath, string contentType, long fileSize, int issueId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty.");

        return new Attachment
        {
            FileName = fileName,
            StoragePath = storagePath,
            ContentType = contentType,
            FileSize = fileSize,
            IssueId = issueId,
            CreatedAt = DateTime.UtcNow
        };
    }
}