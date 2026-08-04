using ProjectPlanner.Application.Common.Dtos.Sprint;

namespace ProjectPlanner.Application.Services;

public interface ISprintService
{
    Task<SprintDto> CreateAsync(CreateSprintDto dto, CancellationToken cancellationToken = default);
    Task<SprintDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SprintDto>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<SprintDto> UpdateAsync(int id, UpdateSprintDto dto, CancellationToken cancellationToken = default);
    Task<SprintDto> StartSprintAsync(int id, CancellationToken cancellationToken = default);
    Task<SprintDto> CompleteSprintAsync(int id, int? targetSprintIdForRollover = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}