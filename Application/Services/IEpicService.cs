using ProjectPlanner.Application.Common.Dtos.Epic;

namespace ProjectPlanner.Application.Services;

public interface IEpicService
{
    Task<EpicDto> CreateAsync(CreateEpicDto dto, CancellationToken cancellationToken = default);
    Task<EpicDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EpicDto?> GetByIdWithIssuesAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EpicSummaryDto>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default);
    Task<EpicDto> UpdateAsync(int id, UpdateEpicDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}