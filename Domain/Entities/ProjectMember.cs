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
    public static ProjectMember Create(
    int projectId,
    int userId,
    ProjectRole role,
    int? addedByUserId = null)
    {
        return new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow,
            AddedByUserId = addedByUserId
        };
    }

    public void ChangeRole(ProjectRole role, int? updatedByUserId = null)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;
    }

    public void SoftDelete(int? updatedByUserId = null)
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;
    }

    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
