namespace Domain.Entities;

public class IssueHistory
{
    public int Id { get; set; }

    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public int ChangedByUserId { get; set; }
    public User ChangedByUser { get; set; } = null!;

    public string Field { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}