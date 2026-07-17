using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common;
using ProjectPlanner.Application.Common.Dto.Development;
using ProjectPlanner.Application.Common.Dto.Issue;
using ProjectPlanner.Application.Common.Dto.Project;
using ProjectPlanner.Application.Common.Dto.Shared;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using System.Data;

namespace ProjectPlanner.Application.Services.Implementations;

public class ProjectService(
    IUserRepository userRepository,
    IProjectRepository projectRepository,
    IProjectMemberRepository projectMemberRepository,
    ISprintRepository sprintRepository,
    IIssueRepository issueRepository,
    IHistoryRepository historyRepository,
    IUnitOfWork unitOfWork) : IProjectService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IProjectMemberRepository _projectMemberRepository = projectMemberRepository;
    private readonly ISprintRepository _sprintRepository = sprintRepository;
    private readonly IIssueRepository _issueRepository = issueRepository;
    private readonly IHistoryRepository _historyRepository = historyRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ProjectDashboardResponseDto>> GetUserProjectsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var projects = await _projectRepository.GetProjectsByUserIdAsync(userId, cancellationToken);

        return projects.Select(p => new ProjectDashboardResponseDto(
            p.Id, p.Name, p.Description, p.Key, p.LeadId, p.CreatedAt, p.RowVersion));
    }

    public async Task<ProjectDashboardResponseDto> CreateProjectAsync(int userId, CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Create the Project
        if (await _projectRepository.ExistsByKeyAsync(dto.Key, cancellationToken))
            throw new InvalidOperationException($"A project with key '{dto.Key}' already exists.");

        var project = Project.Create(dto.Name, dto.Key, userId, userId);
        await _projectRepository.AddAsync(project, cancellationToken);

        // 2. Create the Backlog
        var backlogSprint = Sprint.CreateBacklog(project.Id);
        await _sprintRepository.AddAsync(backlogSprint, cancellationToken);

        // 3. Add Creator as a Member (Calling AddMemberAsync)
        var member = ProjectMember.Create(
            projectId: project.Id,
            userId: userId,
            role: ProjectRole.Manager,
            addedByUserId: userId);

        await _projectMemberRepository.AddAsync(member, cancellationToken);

        // 4. Save everything in one go
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProjectDashboardResponseDto(
            project.Id,
            project.Name,
            project.Description,
            project.Key,
            project.LeadId,
            project.CreatedAt,
            project.RowVersion);
    }

    // --- 2.3 Get Project Summary ---
    public async Task<ProjectSummaryDto?> GetProjectSummaryAsync(
    int projectId,
    CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (project is null)
            return null;

        var lead = await _userRepository.GetByIdAsync(project.LeadId, cancellationToken);

        var leadName = lead?.Username ?? "Unknown";

        var sprints = await _sprintRepository.GetByProjectIdAsync(projectId, cancellationToken);

        var issues = await _issueRepository.GetByProjectIdAsync(projectId, cancellationToken);

        var history = await _historyRepository.GetByProjectIdAsync(projectId, 20, cancellationToken);

        var activeSprintCount = sprints.Count(s => s.Status == SprintStatus.Active);

        var totalIssues = issues.Count();

        var doneIssues = issues.Count(i => i.Status == IssueStatus.Done);

        var openIssues = totalIssues - doneIssues;

        var recentActivities = history
            .OrderByDescending(h => h.ChangedAt)
            .Take(10)
            .Select(h => new RecentActivityDto(
                h.Id,
                $"{h.Field} changed",
                h.ChangedByUser.Username,
                h.ChangedAt))
            .ToList();

        return new ProjectSummaryDto(
            ProjectId: project.Id,
            Name: project.Name,
            OwnerName: leadName,
            ActiveSprintCount: activeSprintCount,
            TotalIssuesCount: totalIssues,
            OpenIssuesCount: openIssues,
            DoneIssuesCount: doneIssues,
            RecentActivities: recentActivities);
    }

    // --- 2.4 Get Project Timeline ---
    public async Task<ProjectTimelineDto?> GetProjectTimelineAsync(int projectId, CancellationToken cancellationToken = default)
    {
        if (!await _projectRepository.ExistsAsync(projectId, cancellationToken)) return null;

        var mockSprints = new List<TimelineItemDto>
        {
            new(101, "Sprint 1 Core Baseline", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(9), "Current"),
            new(102, "Sprint 2 Architecture Extensions", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(24), "Future")
        };

        var mockEpics = new List<TimelineItemDto>
        {
            new(501, "Epic: Database Storage Refactoring Engine", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(24), "Active")
        };

        return new ProjectTimelineDto(projectId, mockSprints, mockEpics);
    }

    // --- 2.5 Get Project Backlog ---
    public async Task<ProjectBacklogDto?> GetProjectBacklogAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null) return null;

        var unassigned = new List<BacklogIssueDto>
        {
            new(201, $"{project.Key}-14", "Design authentication context schema wireframes", IssuePriority.High, IssueType.Task),
            new(202, $"{project.Key}-15", "Investigate CORS handshake preflight drops on dashboard load", IssuePriority.Critical, IssueType.Bug)
        };

        var futureSprints = new List<BacklogSprintDto>
        {
            new(102, "Sprint 2 Architecture Extensions", 5, DateTime.UtcNow.AddDays(10))
        };

        return new ProjectBacklogDto(projectId, unassigned, futureSprints);
    }

    public async Task<ProjectBoardDto?> GetProjectBoardAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null) return null;

        var columns = new List<BoardColumnDto>
        {
            new(IssueStatus.ToDo, [
                new(301, $"{project.Key}-5", "Draft project documentation manifest", "Alice Smith", null, IssuePriority.Medium)
            ]),
            new(IssueStatus.InProgress, [
                new(302, $"{project.Key}-9", "Integrate JWT storage logic to local state hook client", "Bob Johnson", null, IssuePriority.High)
            ]),
            new(IssueStatus.Done, [
                new(303, $"{project.Key}-1", "Setup base entity model schemas core files", "Charlie Brown", null, IssuePriority.Low)
            ])
        };

        return new ProjectBoardDto(projectId, columns);
    }

    public async Task<IEnumerable<ProjectCalendarEventDto>> GetProjectCalendarAsync(int projectId, CancellationToken cancellationToken = default)
    {
        if (!await _projectRepository.ExistsAsync(projectId, cancellationToken))
            return [];

        return
        [
            new(301, "PROJ-5", "Draft project documentation manifest", DateTime.UtcNow, DateTime.UtcNow.AddDays(3), "#3b82f6"),
            new(302, "PROJ-9", "Integrate JWT storage logic", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(2), "#ef4444")
        ];
    }

    // --- 2.8 Get Project List ---
    public async Task<IEnumerable<ProjectIssueListDto>> GetProjectListAsync(int projectId, CancellationToken cancellationToken = default)
    {
        if (!await _projectRepository.ExistsAsync(projectId, cancellationToken))
            return [];

        return
        [
            new(301, "PROJ-5", "Draft project documentation manifest", IssueStatus.ToDo, IssuePriority.Medium, "Task", "Alice Smith", DateTime.UtcNow),
            new(302, "PROJ-9", "Integrate JWT storage logic", IssueStatus.InProgress, IssuePriority.High, "Task", "Bob Johnson", DateTime.UtcNow)
        ];
    }


    public async Task<ProjectDevelopmentDto?> GetProjectDevelopmentDetailsAsync(int projectId, CancellationToken cancellationToken = default)
    {
        if (!await _projectRepository.ExistsAsync(projectId, cancellationToken)) return null;

        var branches = new List<BranchDto>
        {
        };

        var pullRequests = new List<IssuePullRequestDto>
        {
        };

        return new ProjectDevelopmentDto(projectId, 1, branches, pullRequests);
    }

    // --- 2.10 Get Archived Work ---
    public async Task<ArchivedWorkDto> GetArchivedWorkAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var archivedIssues = new List<ProjectIssueListDto>
        {
            new(
                901,
                "PROJ-3",
                "Stale feature brainstorm session notes layout",
                IssueStatus.Done,
                IssuePriority.Low,
                "Task",
                "Unassigned",
                DateTime.UtcNow.AddDays(-20)
            )
        };

        return new ArchivedWorkDto(projectId, archivedIssues, []);
    }

    public async Task<ProjectSettingsDto?> GetProjectSettingsAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null) return null;

        var acl = new List<ProjectMemberPermissionDto>
        {
            new(project.LeadId, "Workspace Owner", "lead@projectplanner.com", ProjectRole.Manager)
        };

        return new ProjectSettingsDto(
            project.Id,
            project.Name,
            project.Description,
            false,
            acl,
            project.RowVersion
        );
    }

    public async Task<ProjectSettingsDto> UpdateProjectSettingsAsync(
    int projectId,
    UpdateProjectSettingsDto dto,
    uint clientRowVersion,
    CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, ct)
            ?? throw new NotFoundException($"Project {projectId} not found.");

        if (project.RowVersion != clientRowVersion)
            throw new ConcurrencyException("Project was updated by another user. Refresh required.");

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException("Project was updated concurrently.");
        }

        return new ProjectSettingsDto(
            project.Id,
            project.Name,
            project.Description,
            false,
            [],
            project.RowVersion
        );
    }
}