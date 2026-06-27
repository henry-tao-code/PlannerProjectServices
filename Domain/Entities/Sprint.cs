using Domain.Enums;

namespace Domain.Entities;

public class Sprint
{
    public int Id { get; init; }

    public string Name { get; private set; } = null!;
    public string? Goal { get; private set; }

    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }

    public SprintStatus Status { get; private set; } = SprintStatus.Planned;

    public int ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    private readonly List<Issue> _issues = [];
    public IReadOnlyCollection<Issue> Issues => _issues;

    public int? CreatedBy { get; private set; }
    public int? CompletedBy { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public uint RowVersion { get; set; }

    private void Touch() => UpdatedAt = DateTime.UtcNow;

    public static Sprint Create(
         int projectId,
         int sprintCount,
         string? goal = null,
         DateTime? startDate = null,
         DateTime? endDate = null)
    {
        return new Sprint
        {
            ProjectId = projectId,
            Name = $"Sprint {sprintCount}",
            Goal = goal,
            StartDate = startDate,
            EndDate = endDate,
            Status = SprintStatus.Planned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Sprint CreateBacklog(int projectId)
    {
        return new Sprint
        {
            ProjectId = projectId,
            Name = "Backlog",
            Goal = "Default location for unscheduled issues.",
            Status = SprintStatus.Backlog,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public void Start()
    {
        if (Status != SprintStatus.Planned)
            throw new InvalidOperationException("Only planned sprints can be started.");

        Status = SprintStatus.Active;
        StartDate = DateTime.UtcNow;
        Touch();
    }

    public void Complete(int? completedBy = null)
    {
        if (Status != SprintStatus.Active)
            throw new InvalidOperationException("Only active sprints can be completed.");

        Status = SprintStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        EndDate ??= DateTime.UtcNow;
        CompletedBy = completedBy;

        Touch();
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Sprint name cannot be empty.");

        Name = name.Trim();
        Touch();
    }

    public void UpdateGoal(string? goal)
    {
        Goal = goal;
        Touch();
    }

    public void Schedule(DateTime? startDate, DateTime? endDate)
    {
        if (endDate.HasValue && startDate.HasValue && endDate < startDate)
            throw new InvalidOperationException("End date cannot be before start date.");

        StartDate = startDate?.ToUniversalTime();
        EndDate = endDate?.ToUniversalTime();

        Touch();
    }

    public void Delete()
    {
        IsDeleted = true;
        Touch();
    }
}