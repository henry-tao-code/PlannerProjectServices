using Domain.Entities;
using Domain.Enums;
using ProjectPlanner.Application.Common.Dto.Issue;
using ProjectPlanner.Application.Common.Dto.Sprint;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class SprintService(
    IProjectRepository projectRepository,
    ISprintRepository sprintRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ISprintService
{

    public async Task<SprintDto> CreateAsync(
        CreateSprintDto dto,
        CancellationToken ct = default)
    {
        if (!await projectRepository.ExistsAsync(dto.ProjectId, ct))
            throw new KeyNotFoundException($"Project {dto.ProjectId} not found.");

        var sprints = await sprintRepository.GetByProjectIdAsync(dto.ProjectId, ct);
        var sprintCount = sprints.Count() + 1;

        var sprint = Sprint.Create(
            dto.ProjectId,
            sprintCount,
            dto.Goal,
            dto.StartDate,
            dto.EndDate
        );

        await sprintRepository.AddAsync(sprint, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return MapToDto(sprint);
    }

    // ---------------- GET ----------------

    public async Task<SprintDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var sprint = await sprintRepository.GetByIdAsync(id, ct);
        return sprint is null ? null : MapToDto(sprint);
    }

    public async Task<IEnumerable<SprintDto>> GetByProjectIdAsync(
        int projectId,
        CancellationToken ct = default)
    {
        if (!await projectRepository.ExistsAsync(projectId, ct))
            throw new KeyNotFoundException($"Project {projectId} not found.");

        var sprints = await sprintRepository.GetByProjectIdAsync(projectId, ct);
        return sprints.Select(MapToDto);
    }

    // ---------------- UPDATE ----------------

    public async Task<SprintDto> UpdateAsync(
        int id,
        UpdateSprintDto dto,
        CancellationToken ct = default)
    {
        var sprint = await sprintRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Sprint {id} not found.");

        sprint.Rename(dto.Name);
        sprint.UpdateGoal(dto.Goal);

        sprint.Schedule(dto.StartDate?.ToUniversalTime(), dto.EndDate?.ToUniversalTime());

        await unitOfWork.SaveChangesAsync(ct);

        return MapToDto(sprint);
    }

    // ---------------- START ----------------

    public async Task<SprintDto> StartSprintAsync(int id, CancellationToken ct = default)
    {
        var sprint = await sprintRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Sprint {id} not found.");

        sprint.Start();

        await unitOfWork.SaveChangesAsync(ct);

        return MapToDto(sprint);
    }

    // ---------------- COMPLETE ----------------

    public async Task<SprintDto> CompleteSprintAsync(
    int id,
    int? targetSprintIdForRollover = null,
    CancellationToken ct = default)
    {
        var sprint = await sprintRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Sprint {id} not found.");

        var userId = userContext.UserId;

        Sprint? targetSprint = null;

        if (targetSprintIdForRollover.HasValue)
        {
            targetSprint = await sprintRepository.GetByIdAsync(targetSprintIdForRollover.Value, ct)
                ?? throw new KeyNotFoundException("Target sprint not found.");
        }

        var issues = sprint.Issues.ToList();

        foreach (var issue in issues)
        {
            // Close unfinished issues
            if (issue.Status != IssueStatus.Done)
            {
                issue.UpdateStatus(IssueStatus.Done, userId);
            }

            // Move all issues
            issue.MoveToSprint(targetSprint?.Id, userId);
        }

        sprint.Complete();

        await unitOfWork.SaveChangesAsync(ct);

        return MapToDto(sprint);
    }

    // ---------------- DELETE ----------------

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var sprint = await sprintRepository.GetByIdAsync(id, ct);
        if (sprint is null) return false;

        sprint.Delete();

        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    // ---------------- MAPPING ----------------

    private static SprintDto MapToDto(Sprint sprint)
    {
        var issueDtos = sprint.Issues.Select(i => new IssueSummaryDto(
            i.Id,
            i.IssueKey,
            i.Title,
            i.Status,
            i.Priority,
            i.Assignee?.Username,
            i.StoryPoints
        ));

        return new SprintDto(
            sprint.Id,
            sprint.ProjectId,
            sprint.Name,
            sprint.StartDate,
            sprint.EndDate,
            sprint.Status,
            sprint.Goal,
            issueDtos
        );
    }
}