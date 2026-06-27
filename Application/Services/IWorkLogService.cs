using ProjectPlanner.Application.Common.Dto;
using ProjectPlanner.Application.Common.Dto.WorkLog;

namespace ProjectPlanner.Application.Services;

public interface IWorkLogService
{
    Task<WorkLogDto> CreateAsync(CreateWorkLogDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkLogDto>> GetByIssueIdAsync(int issueId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}