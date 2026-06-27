using Domain.Enums;

namespace Domain.Entities;

public class ProjectMember
{
    // --- Composite Key ---
    public int ProjectId { get; init; }
    public Project Project { get; init; } = null!;

    public int UserId { get; init; }
    public User User { get; init; } = null!;

    // --- Membership ---
    public ProjectRole Role { get; set; }

    public bool IsDeleted { get; set; }

    // --- Audit (IMPORTANT addition) ---
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int? AddedByUserId { get; set; }
    public int? UpdatedByUserId { get; set; }

    // --- Domain behavior (recommended) ---
    public void ChangeRole(ProjectRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Remove()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
