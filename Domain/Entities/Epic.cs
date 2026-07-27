using Domain.Enums;

namespace Domain.Entities;

public class Epic
{
    public int Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Summary { get; private set; } = null!;
    public string? Description { get; private set; }
    public EpicStatus Status { get; private set; } = EpicStatus.ToDo;
    public DateTime? StartDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public int ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public int? AssigneeId { get; private set; }
    public User? Assignee { get; private set; }
    public ICollection<Issue> Issues { get; private set; } = [];
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public uint RowVersion { get; private set; }

    public static Epic Create(
        string title,
        string summary,
        int projectId,
        string? description = null,
        int? assigneeId = null,
        DateTime? startDate = null,
        DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Epic title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Epic summary cannot be empty.", nameof(summary));

        if (startDate.HasValue && dueDate.HasValue && startDate > dueDate)
            throw new InvalidOperationException("Start date cannot be later than the due date.");

        return new Epic
        {
            Title = title,
            Summary = summary,
            Description = description,
            ProjectId = projectId,
            AssigneeId = assigneeId,
            Status = EpicStatus.ToDo,
            StartDate = startDate,
            DueDate = dueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public void UpdateDetails(string title, string summary, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Epic title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Epic summary cannot be empty.", nameof(summary));

        Title = title;
        Summary = summary;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTimeline(DateTime? startDate, DateTime? dueDate)
    {
        if (startDate.HasValue && dueDate.HasValue && startDate > dueDate)
            throw new InvalidOperationException("Start date cannot be later than the due date.");

        StartDate = startDate;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(EpicStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAssignee(int? assigneeId)
    {
        AssigneeId = assigneeId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        if (IsDeleted) return;

        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}