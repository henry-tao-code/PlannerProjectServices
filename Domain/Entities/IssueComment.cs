using Domain.Entities;

public class IssueComment
{
    public int Id { get; set; }

    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public string Body { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }

    public bool IsEdited => EditedAt.HasValue;
    public uint RowVersion { get; set; }
}