using ProjectPlanner.Application.Common.Dto;
using ProjectPlanner.Application.Common.Dto.Project;

namespace ProjectPlanner.Application.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDashboardResponseDto>> GetUserProjectsAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<ProjectDashboardResponseDto> CreateProjectAsync(
        int userId,
        CreateProjectDto dto,
        CancellationToken cancellationToken = default);

    Task<ProjectSummaryDto?> GetProjectSummaryAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<ProjectTimelineDto?> GetProjectTimelineAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<ProjectBacklogDto?> GetProjectBacklogAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<ProjectBoardDto?> GetProjectBoardAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectCalendarEventDto>> GetProjectCalendarAsync(
        int projectId,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ProjectIssueListDto>> GetProjectListAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<ProjectDevelopmentDto?> GetProjectDevelopmentDetailsAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<ArchivedWorkDto> GetArchivedWorkAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<ProjectSettingsDto?> GetProjectSettingsAsync(
        int projectId,
        CancellationToken cancellationToken = default);

    Task<ProjectSettingsDto> UpdateProjectSettingsAsync(
        int projectId,
        UpdateProjectSettingsDto dto,
        uint clientRowVersion,
        CancellationToken cancellationToken = default);
}