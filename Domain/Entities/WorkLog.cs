namespace Domain.Entities;

public class WorkLog
{
    public int Id { get; init; }

    // --- Relationships ---
    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // --- Time tracking ---
    public int TimeSpentMinutes { get; set; }
    public string? Description { get; set; }
    public DateTime StartedAt { get; set; }

    // --- Audit ---
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}