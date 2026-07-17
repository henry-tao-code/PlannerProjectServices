using Domain.Entities;
using ProjectPlanner.Application.Common.Dto.Epic;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class EpicService(
    IEpicRepository epicRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IEpicService
{
    public async Task<EpicDto> CreateAsync(CreateEpicDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Validate that the target project exists
        if (!await projectRepository.ExistsAsync(dto.ProjectId, cancellationToken))
            throw new KeyNotFoundException($"Project with ID {dto.ProjectId} was not found.");

        // 2. Resolve assignee username early if an assignee is provided
        var assigneeUsername = await GetUsernameAsync(dto.AssigneeId, cancellationToken);

        // 3. Instantiate the Epic using its DDD domain factory method (encapsulates validation)
        var epic = Epic.Create(
            dto.Name,
            dto.Summary,
            dto.ProjectId,
            dto.Description,
            dto.AssigneeId,
            dto.StartDate,
            dto.DueDate
        );

        await epicRepository.AddAsync(epic, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(epic, assigneeUsername);
    }

    public async Task<EpicDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var epic = await epicRepository.GetByIdAsync(id, cancellationToken);
        if (epic == null) return null;

        var assigneeUsername = epic.Assignee?.Username ?? await GetUsernameAsync(epic.AssigneeId, cancellationToken);

        return MapToDto(epic, assigneeUsername);
    }

    public async Task<EpicDto?> GetByIdWithIssuesAsync(int id, CancellationToken cancellationToken = default)
    {
        var epic = await epicRepository.GetByIdWithIssuesAsync(id, cancellationToken);
        if (epic == null) return null;

        var assigneeUsername = epic.Assignee?.Username ?? await GetUsernameAsync(epic.AssigneeId, cancellationToken);

        return MapToDto(epic, assigneeUsername);
    }

    public async Task<IEnumerable<EpicSummaryDto>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        if (!await projectRepository.ExistsAsync(projectId, cancellationToken))
            throw new KeyNotFoundException($"Project with ID {projectId} was not found.");

        var epics = await epicRepository.GetByProjectIdAsync(projectId, cancellationToken);

        return epics.Select(e => new EpicSummaryDto(
            e.Id,
            e.Name,
            e.Summary,
            e.Status,
            e.Assignee?.Username,
            e.Issues.Count // Derived directly from eager-loaded collection count
        ));
    }

    public async Task<EpicDto> UpdateAsync(int id, UpdateEpicDto dto, CancellationToken cancellationToken = default)
    {
        var epic = await epicRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Epic with ID {id} was not found.");

        // Optimistic Concurrency Check
        if (epic.RowVersion != dto.RowVersion)
            throw new InvalidOperationException("The record was modified by another request. Refresh your data.");

        // Execute self-contained DDD methods on the entity
        epic.UpdateDetails(dto.Name, dto.Summary, dto.Description);
        epic.UpdateTimeline(dto.StartDate, dto.DueDate);
        epic.UpdateStatus(dto.Status);
        epic.SetAssignee(dto.AssigneeId);

        epicRepository.Update(epic);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var assigneeUsername = await GetUsernameAsync(epic.AssigneeId, cancellationToken);

        return MapToDto(epic, assigneeUsername);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var epic = await epicRepository.GetByIdAsync(id, cancellationToken);
        if (epic == null) return false;

        // Execute soft delete behavior on domain layer instead of context.Remove()
        epic.Delete();

        epicRepository.Update(epic);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    // --- Private DRY Helpers ---

    private async Task<string?> GetUsernameAsync(int? userId, CancellationToken ct)
    {
        if (!userId.HasValue) return null;
        var user = await userRepository.GetByIdAsync(userId.Value, ct);
        return user?.Username;
    }

    private static EpicDto MapToDto(Epic epic, string? assigneeUsername)
    {
        return new EpicDto(
            epic.Id,
            epic.Name,
            epic.Summary,
            epic.Description,
            epic.Status,
            epic.StartDate,
            epic.DueDate,
            epic.ProjectId,
            epic.AssigneeId,
            assigneeUsername,
            epic.CreatedAt,
            epic.UpdatedAt,
            epic.RowVersion
        );
    }
}