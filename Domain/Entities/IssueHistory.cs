namespace Domain.Entities;

public class IssueHistory
{
    public int Id { get; set; }

    // --- Relationships ---
    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public int ChangedByUserId { get; set; }
    public User ChangedByUser { get; set; } = null!;

    // --- Change data ---
    public string Field { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    // --- Audit ---
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}