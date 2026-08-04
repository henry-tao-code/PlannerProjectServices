using Domain.Entities;
using ProjectPlanner.Application.Common.Dtos.WorkLog;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class WorkLogService(
    IWorkLogRepository workLogRepository,
    IIssueRepository issueRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IWorkLogService
{
    public async Task<WorkLogDto> CreateAsync(CreateWorkLogDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(dto.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException($"User with ID {dto.UserId} was not found.");

        var workLog = new WorkLog
        {
            IssueId = dto.IssueId,
            UserId = dto.UserId,
            TimeSpentMinutes = dto.TimeSpentMinutes,
            Description = dto.Description,
            StartedAt = dto.StartedAt.ToUniversalTime(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await workLogRepository.AddAsync(workLog, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(workLog, user.Username);
    }

    public async Task<IEnumerable<WorkLogDto>> GetByIssueIdAsync(int issueId, CancellationToken cancellationToken = default)
    {
        // Ensure parent tracking target exists
        if (!await issueRepository.ExistsAsync(issueId, cancellationToken))
            throw new KeyNotFoundException($"Issue with ID {issueId} was not found.");

        var logs = await workLogRepository.GetByIssueIdAsync(issueId, cancellationToken);
        return logs.Select(w => MapToDto(w, w.User?.Username ?? "Unknown User"));
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var workLog = await workLogRepository.GetByIdAsync(id, cancellationToken);
        if (workLog == null) return false;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static WorkLogDto MapToDto(WorkLog w, string username)
    {
        return new WorkLogDto(
            w.Id,
            w.IssueId,
            w.UserId,
            username,
            w.TimeSpentMinutes,
            w.Description,
            w.StartedAt,
            w.CreatedAt
        );
    }
}