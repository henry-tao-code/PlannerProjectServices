using Domain.Entities;
using Domain.Enums;
using ProjectPlanner.Application.Common.Dto.Issue;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class IssueService(
    IIssueRepository issueRepository,
    IProjectRepository projectRepository,
    IEpicRepository epicRepository,
    ISprintRepository sprintRepository,
    IUserRepository userRepository,
    ISearchIndexService searchIndexService,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IIssueService
{
    public async Task<IssueDetailDto> CreateAsync(IssueCreateDto dto, CancellationToken ct = default)
    {
        var project = await projectRepository.GetByIdAsync(dto.ProjectId, ct)
            ?? throw new KeyNotFoundException($"Project {dto.ProjectId} not found.");

        var sprintId = await ResolveSprintAsync(dto.ProjectId, dto.SprintId, ct);

        var issueCount = await issueRepository.GetCountByProjectIdAsync(dto.ProjectId, ct);
        var issueKey = $"{project.Key}-{issueCount + 1}";

        var userId = userContext.UserId;

        var issue = Issue.Create(
            issueKey: issueKey,
            title: dto.Title,
            projectId: dto.ProjectId,
            description: dto.Description,
            priority: dto.Priority,
            issueType: dto.IssueType
        );

        issue.MoveToSprint(sprintId, userId);
        issue.SetAssignee(dto.AssigneeId);
        issue.SetReporter(dto.ReporterId);
        issue.SetDueDate(dto.DueDate);

        foreach (var label in dto.Labels ?? [])
        {
            issue.AddLabel(label);
        }

        await issueRepository.AddAsync(issue, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await searchIndexService.IndexIssueAsync(issue.Id, ct);

        return await MapAsync(issue, ct);
    }

    public async Task<IssueDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(id, ct);
        return issue is null ? null : await MapAsync(issue, ct);
    }

    public async Task<IEnumerable<IssueSummaryDto>> GetByProjectAsync(int projectId, CancellationToken ct = default)
    {
        if (!await projectRepository.ExistsAsync(projectId, ct))
            throw new KeyNotFoundException($"Project {projectId} not found.");

        var issues = await issueRepository.GetByProjectIdAsync(projectId, ct);

        return issues.Select(i => new IssueSummaryDto(
            i.Id,
            i.IssueKey,
            i.Title,
            i.Epic?.Title,
            i.Status,
            i.Priority,
            i.Assignee?.Username,
            i.StoryPoints
        ));
    }

    public async Task<IEnumerable<IssueSummaryDto>> SearchIssuesAsync(
    int projectId,
    string searchTerm,
    CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return [];
        }

        var issues = await issueRepository.FuzzySearchIssuesAsync(projectId, searchTerm, ct);

        return issues.Select(i => new IssueSummaryDto(
            i.Id,
            i.IssueKey,
            i.Title,
            i.Epic?.Title,
            i.Status,
            i.Priority,
            i.Assignee?.Username,
            i.StoryPoints
        ));
    }

    public async Task<IssueDetailDto> UpdateCoreAsync(
    int id,
    UpdateIssueCoreDto dto,
    CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Issue {id} not found.");

        if (issue.RowVersion != dto.RowVersion)
            throw new InvalidOperationException("Concurrency conflict detected.");

        var userId = userContext.UserId;

        // Core fields
        issue.UpdateDetails(
            dto.Title,
            dto.Description,
            dto.Priority,
            dto.Type,
            dto.StoryPoints
        );

        // Status
        if (dto.Status.HasValue)
            issue.UpdateStatus(dto.Status.Value, userId);

        // Dates
        if (dto.StartDate.HasValue)
            issue.SetStartDate(dto.StartDate.Value);

        if (dto.DueDate.HasValue)
            issue.SetDueDate(dto.DueDate.Value);

        // Assignee / Reporter
        issue.SetAssignee(dto.AssigneeId);
        issue.SetReporter(dto.ReporterId);

        // Sprint / Parent
        issue.SetEpic(dto.EpicId);
        issue.MoveToSprint(dto.SprintId, userId);
        issue.SetParent(dto.ParentIssueId);

        if (dto.Labels is not null)
        {
            var incoming = dto.Labels
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existing = issue.Labels.Select(l => l.Value).ToList();

            foreach (var label in existing)
            {
                if (!incoming.Contains(label))
                    issue.RemoveLabel(label);
            }

            foreach (var label in incoming)
            {
                issue.AddLabel(label);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        await searchIndexService.IndexIssueAsync(issue.Id, ct);

        return await MapAsync(issue, ct);
    }

    public async Task<IssueDetailDto> UpdateAssignmentAsync(int id, UpdateIssueAssignmentDto dto, CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Issue {id} not found.");

        issue.SetAssignee(dto.AssigneeId);
        issue.SetReporter(dto.ReporterId);

        await unitOfWork.SaveChangesAsync(ct);

        return await MapAsync(issue, ct);
    }

    // ---------------- MOVE ----------------

    public async Task<IssueDetailDto> MoveIssueAsync(int issueId, MoveIssueDto dto, CancellationToken ct)
    {
        var issue = await issueRepository.GetByIdAsync(issueId, ct)
            ?? throw new KeyNotFoundException($"Issue {issueId} not found.");

        var userId = userContext.UserId;

        if (dto.TargetStatus.HasValue)
            issue.UpdateStatus(dto.TargetStatus.Value, userId);

        if (dto.TargetSprintId.HasValue)
        {
            var sprintId = await ResolveSprintAsync(issue.ProjectId, dto.TargetSprintId, ct);
            issue.MoveToSprint(sprintId, userId);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return await MapAsync(issue, ct);
    }

    // ---------------- LABELS ----------------

    public async Task<IssueDetailDto> UpdateLabelsAsync(int id, UpdateIssueLabelsDto dto, CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Issue {id} not found.");

        issue.Labels.Clear();

        foreach (var label in dto.Labels)
            issue.AddLabel(label);

        await unitOfWork.SaveChangesAsync(ct);

        return await MapAsync(issue, ct);
    }

    // ---------------- TIME TRACKING ----------------

    public async Task<IssueDetailDto> UpdateTimeTrackingAsync(int id, UpdateIssueTimeTrackingDto dto, CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Issue {id} not found.");

        issue.TimeTracking.OriginalEstimateMinutes = dto.OriginalEstimateMinutes;
        issue.TimeTracking.TimeSpentMinutes = dto.TimeSpentMinutes;
        issue.TimeTracking.TimeRemainingMinutes = dto.TimeRemainingMinutes;

        await unitOfWork.SaveChangesAsync(ct);

        return await MapAsync(issue, ct);
    }

    // ---------------- DELETE ----------------

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(id, ct);
        if (issue is null) return false;

        issueRepository.Remove(issue);
        await unitOfWork.SaveChangesAsync(ct);

        await searchIndexService.RemoveAsync(SearchEntityType.Issue, issue.Id, ct);

        return true;
    }

    // ---------------- HELPERS ----------------

    private async Task<int> ResolveSprintAsync(int projectId, int? sprintId, CancellationToken ct)
    {
        if (sprintId is null || sprintId == 0)
        {
            var backlog = await sprintRepository.GetBacklogSprintByProjectIdAsync(projectId, ct)
                ?? throw new InvalidOperationException("Backlog sprint not found.");

            return backlog.Id;
        }

        var sprint = await sprintRepository.GetByIdAsync(sprintId.Value, ct)
            ?? throw new KeyNotFoundException($"Sprint {sprintId} not found.");

        if (sprint.ProjectId != projectId)
            throw new InvalidOperationException("Sprint does not belong to project.");

        return sprint.Id;
    }

    private async Task<IssueDetailDto> MapAsync(Issue issue, CancellationToken ct)
    {
        var assignee = await GetUsernameAsync(issue.AssigneeId, ct);
        var reporter = await GetUsernameAsync(issue.ReporterId, ct);
        var epic = await (GetEpicName(issue.EpicId, ct));

        return new IssueDetailDto
        {
            Id = issue.Id,
            IssueKey = issue.IssueKey,
            Title = issue.Title,
            Description = issue.Description,

            IssueType = issue.IssueType,
            Status = issue.Status,
            Priority = issue.Priority,

            ProjectId = issue.ProjectId,
            EpicId = issue.EpicId,
            EpicName = epic,
            SprintId = issue.SprintId,

            AssigneeId = issue.AssigneeId,
            AssigneeName = assignee,
            ReporterId = issue.ReporterId,
            ReporterName = reporter,

            StartDate = issue.StartDate,
            DueDate = issue.DueDate,

            ParentIssueId = issue.ParentIssueId,

            StoryPoints = issue.StoryPoints,

            TimeTracking = new IssueTimeTrackingDto
            {
                OriginalEstimateMinutes = issue.TimeTracking.OriginalEstimateMinutes,
                TimeSpentMinutes = issue.TimeTracking.TimeSpentMinutes,
                TimeRemainingMinutes = issue.TimeTracking.TimeRemainingMinutes
            },

            Labels = [.. issue.Labels.Select(l => l.Value)],

            Comments = [],
            History = [],
            Attachments = [],

            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt,
            RowVersion = issue.RowVersion
        };
    }

    private async Task<string?> GetUsernameAsync(int? userId, CancellationToken ct)
    {
        if (!userId.HasValue) return null;

        var user = await userRepository.GetByIdAsync(userId.Value, ct);
        return user?.Username;
    }

    private async Task<string?> GetEpicName(int? epicId, CancellationToken ct)
    {
        if (!epicId.HasValue) return null;

        var epic = await epicRepository.GetByIdAsync(epicId.Value, ct);
        return epic?.Title;
    }
}