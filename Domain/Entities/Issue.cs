using Domain.Enums;

namespace Domain.Entities;

public class Issue
{
    public int Id { get; init; }

    // ---------- Identity ----------
    public string IssueKey { get; private set; } = null!;

    // ---------- Core ----------
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }

    public IssueType IssueType { get; private set; }
    public IssueStatus Status { get; private set; }
    public IssuePriority Priority { get; private set; }

    // ---------- Relationships ----------
    public int ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    public int? SprintId { get; private set; }
    public Sprint? Sprint { get; private set; }

    public int? EpicId { get; private set; }
    public Epic? Epic { get; private set; }

    public int? AssigneeId { get; private set; }
    public User? Assignee { get; private set; }

    public int? ReporterId { get; private set; }
    public User? Reporter { get; private set; }

    // ---------- Hierarchy ----------
    public int? ParentIssueId { get; private set; }
    public Issue? ParentIssue { get; private set; }

    public ICollection<Issue> SubIssues { get; } = [];

    // ---------- Links ----------
    public ICollection<IssueLink> OutgoingLinks { get; } = [];
    public ICollection<IssueLink> IncomingLinks { get; } = [];

    // ---------- Scheduling ----------
    public DateTime? StartDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // ---------- Estimation ----------
    public int StoryPoints { get; private set; } = 0;

    public IssueTimeTracking TimeTracking { get; private set; } = new();

    // ---------- Child Collections ----------
    public ICollection<IssueLabel> Labels { get; } = [];
    public ICollection<IssueComment> Comments { get; } = [];
    public ICollection<IssueHistory> History { get; } = [];
    public ICollection<IssueAttachment> Attachments { get; } = [];

    public ICollection<IssueCommit> Commits { get; } = [];
    public ICollection<IssuePullRequest> PullRequests { get; } = [];

    // ---------- Audit ----------
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public uint RowVersion { get; set; }

    private Issue() { }

    public static Issue Create(
        string issueKey,
        string title,
        int projectId,
        string? description = null,
        IssuePriority priority = IssuePriority.Medium,
        IssueType issueType = IssueType.Task)
    {
        if (string.IsNullOrWhiteSpace(issueKey))
            throw new ArgumentException("Issue key is required.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Issue title is required.");

        return new Issue
        {
            IssueKey = issueKey.Trim().ToUpperInvariant(),
            Title = title.Trim(),
            Description = description,
            ProjectId = projectId,
            Priority = priority,
            IssueType = issueType,
            Status = IssueStatus.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(
    string? title,
    string? description,
    IssuePriority? priority,
    IssueType? issueType,
    int? storyPoints)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title = title.Trim();

        if (description is not null)
            Description = description;

        if (priority.HasValue)
            Priority = priority.Value;

        if (issueType.HasValue)
            IssueType = issueType.Value;

        if (storyPoints.HasValue)
            StoryPoints = storyPoints.Value;

        Touch();
    }

    public void SetAssignee(int? userId)
    {
        AssigneeId = userId;
        Touch();
    }

    public void SetReporter(int? userId)
    {
        ReporterId = userId;
        Touch();
    }

    public void SetEpic(int? Id)
    {
        EpicId = Id;
        Touch();
    }

    public void MoveToSprint(int? sprintId, int changedByUserId)
    {
        if (SprintId == sprintId)
            return;

        var old = SprintId?.ToString();
        var newValue = sprintId?.ToString();

        SprintId = sprintId;

        AddHistory(
            field: "SprintId",
            oldValue: old,
            newValue: newValue,
            changedByUserId: changedByUserId
        );

        Touch();
    }

    public void SetParent(int? ParentIssueId)
    {
        this.ParentIssueId = ParentIssueId;
        Touch();
    }

    public void UpdateStatus(IssueStatus status, int changedByUserId)
    {
        if (Status == status)
            return;

        var old = Status.ToString();

        Status = status;

        AddHistory(
            field: "Status",
            oldValue: old,
            newValue: status.ToString(),
            changedByUserId: changedByUserId
        );

        if (status == IssueStatus.InProgress)
            StartDate ??= DateTime.UtcNow;

        if (status == IssueStatus.Done)
            CompletedAt = DateTime.UtcNow;
        else
            CompletedAt = null;

        Touch();
    }

    public void SetStartDate(DateTime? startDate)
    {
        StartDate = startDate;
        Touch();
    }

    public void SetDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
        Touch();
    }

    public void UpdateTimeTracking(
        int? originalEstimateMinutes,
        int? timeSpentMinutes,
        int? timeRemainingMinutes)
    {
        TimeTracking.OriginalEstimateMinutes = originalEstimateMinutes;
        TimeTracking.TimeSpentMinutes = timeSpentMinutes;
        TimeTracking.TimeRemainingMinutes = timeRemainingMinutes;

        Touch();
    }

    public void AddLabel(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        value = value.Trim();

        if (Labels.Any(l =>
            l.Value.Equals(value, StringComparison.OrdinalIgnoreCase)))
            return;

        Labels.Add(new IssueLabel(value));

        Touch();
    }

    public void RemoveLabel(string value)
    {
        var label = Labels.FirstOrDefault(l =>
            l.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

        if (label is null)
            return;

        Labels.Remove(label);

        Touch();
    }

    public void AddSubIssue(Issue subIssue)
    {
        ArgumentNullException.ThrowIfNull(subIssue);

        subIssue.ParentIssueId = Id;

        SubIssues.Add(subIssue);

        Touch();
    }

    public void Delete()
    {
        IsDeleted = true;
        Touch();
    }

    private void AddHistory(string field, string? oldValue, string? newValue, int changedByUserId)
    {
        History.Add(new IssueHistory
        {
            Field = field,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedByUserId = changedByUserId,
            IssueId = Id
        });
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

public class IssueTimeTracking
{
    public int? OriginalEstimateMinutes { get; set; }
    public int? TimeSpentMinutes { get; set; }
    public int? TimeRemainingMinutes { get; set; }

    public double? ProgressPercentage =>
        OriginalEstimateMinutes.HasValue && OriginalEstimateMinutes > 0 &&
        TimeSpentMinutes.HasValue
            ? (double)TimeSpentMinutes.Value / OriginalEstimateMinutes.Value * 100
            : null;

    public bool IsOverEstimated =>
        TimeSpentMinutes.HasValue &&
        OriginalEstimateMinutes.HasValue &&
        TimeSpentMinutes > OriginalEstimateMinutes;
}