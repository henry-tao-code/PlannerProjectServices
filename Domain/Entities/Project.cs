namespace Domain.Entities;

public class Project
{
    public int Id { get; init; }

    public string Name { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string? Description { get; set; }
    public string Slug { get; set; } = null!;

    public int LeadId { get; set; }
    public User Lead { get; set; } = null!;

    public bool IsArchived { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public int CreatedByUserId { get; set; }
    public int? UpdatedByUserId { get; set; }
    public uint RowVersion { get; set; }

    public ICollection<ProjectMember> ProjectMembers { get; set; } = [];
    public ICollection<Sprint> Sprints { get; set; } = [];
    public ICollection<Epic> Epics { get; set; } = [];
    public ICollection<Issue> Issues { get; set; } = [];
    public ProjectGitHubRepository? GitHubRepository { get; private set; }

    public static Project Create(string name, string key, int leadId, int createdByUserId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Project key cannot be empty.", nameof(key));

        var normalizedKey = key.Trim().ToUpperInvariant();

        return new Project
        {
            Name = name.Trim(),
            Key = normalizedKey,
            Slug = GenerateSlug(name, normalizedKey),
            LeadId = leadId,
            CreatedByUserId = createdByUserId,
            Description = description,

            IsArchived = false,
            IsDeleted = false,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // --- Domain behaviors ---
    public void Archive()
    {
        IsArchived = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsArchived = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty.", nameof(name));

        Name = name.Trim();
        Slug = GenerateSlug(name, Key);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeLead(int leadId)
    {
        LeadId = leadId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // --- Helper ---
    private static string GenerateSlug(string name, string key)
    {
        var baseSlug = $"{key}-{name}"
            .ToLowerInvariant()
            .Replace(" ", "-");

        return baseSlug;
    }
}