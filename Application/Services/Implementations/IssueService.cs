using Domain.Entities;
using Domain.Enums;
using ProjectPlanner.Application.Common.Dto.Issue;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class IssueService(
    IIssueRepository issueRepository,
    ISprintRepository sprintRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IHistoryService historyService,
    IUnitOfWork unitOfWork) : IIssueService
{
    public async Task<IssueDetailDto> CreateAsync(IssueCreateDto dto, CancellationToken ct = default)
    {
        var project = await projectRepository.GetByIdAsync(dto.ProjectId, ct)
            ?? throw new KeyNotFoundException($"Project {dto.ProjectId} not found.");

        var sprintId = await ResolveSprintAsync(dto.ProjectId, dto.SprintId, ct);

        var issueCount = await issueRepository.GetCountByProjectIdAsync(dto.ProjectId, ct);
        var issueKey = $"{project.Key}-{issueCount + 1}";

        var issue = Issue.Create(
            issueKey: issueKey,
            title: dto.Title,
            projectId: dto.ProjectId,
            description: dto.Description,
            priority: dto.Priority,
            issueType: dto.IssueType
        );

        issue.MoveToSprint(sprintId);
        issue.AssignTo(dto.AssigneeId);
        issue.SetReporter(dto.ReporterId);
        issue.SetDueDate(dto.DueDate);

        foreach (var label in dto.Labels ?? [])
        {
            issue.AddLabel(label);
        }

        await issueRepository.AddAsync(issue, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return await MapAsync(issue, ct);
    }

    // ---------------- READ ----------------

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
            i.Status,
            i.Priority,
            i.Assignee?.Username,
            i.StoryPoints
        ));
    }

    public async Task<IssueDetailDto> UpdateCoreAsync(int id, UpdateIssueCoreDto dto, CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Issue {id} not found.");

        if (issue.RowVersion != dto.RowVersion)
            throw new InvalidOperationException("Concurrency conflict detected.");

        issue.UpdateDetails(
            dto.Title,
            dto.Description,
            dto.Priority,
            dto.Type,
            dto.StoryPoints
        );

        if (dto.Status.HasValue)
            issue.UpdateStatus(dto.Status.Value);

        if (dto.DueDate.HasValue)
            issue.SetDueDate(dto.DueDate.Value);

        if (dto.StartDate.HasValue)
            issue.SetStartDate(dto.StartDate.Value);

        await unitOfWork.SaveChangesAsync(ct);

        return await MapAsync(issue, ct);
    }

    public async Task<IssueDetailDto> UpdateAssignmentAsync(int id, UpdateIssueAssignmentDto dto, CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Issue {id} not found.");

        issue.AssignTo(dto.AssigneeId);
        issue.SetReporter(dto.ReporterId);

        await unitOfWork.SaveChangesAsync(ct);

        return await MapAsync(issue, ct);
    }

    // ---------------- MOVE ----------------

    public async Task<IssueDetailDto> MoveIssueAsync(int issueId, MoveIssueDto dto, CancellationToken ct)
    {
        var issue = await issueRepository.GetByIdAsync(issueId, ct)
            ?? throw new KeyNotFoundException($"Issue {issueId} not found.");

        if (dto.TargetStatus.HasValue)
            issue.UpdateStatus(dto.TargetStatus.Value);

        if (dto.TargetSprintId.HasValue)
        {
            var sprintId = await ResolveSprintAsync(issue.ProjectId, dto.TargetSprintId, ct);
            issue.MoveToSprint(sprintId);
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
            SprintId = issue.SprintId,

            AssigneeId = issue.AssigneeId,
            ReporterId = issue.ReporterId,

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

            Labels = issue.Labels.Select(l => l.Value).ToList(),

            Comments = [],
            History = [],
            Attachments = [],

            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt
        };
    }

    private async Task<string?> GetUsernameAsync(int? userId, CancellationToken ct)
    {
        if (!userId.HasValue) return null;

        var user = await userRepository.GetByIdAsync(userId.Value, ct);
        return user?.Username;
    }
}