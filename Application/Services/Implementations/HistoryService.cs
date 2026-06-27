using Domain.Entities;
using ProjectPlanner.Application.Common.Dto.History;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class HistoryService(
    IHistoryRepository historyRepository,
    IUnitOfWork unitOfWork) : IHistoryService
{
    public async Task<IEnumerable<HistoryDto>> GetByIssueIdAsync(
        int issueId,
        CancellationToken ct = default)
    {
        var history = await historyRepository.GetByIssueIdAsync(issueId, ct);

        return history.Select(h => new HistoryDto(
            h.Id,
            h.IssueId,
            h.Field,
            h.OldValue,
            h.NewValue,
            h.ChangedByUserId,
            h.ChangedAt
        ));
    }

    public async Task LogChangeAsync(
        CreateHistoryDto dto,
        CancellationToken ct = default)
    {
        var entry = new IssueHistory
        {
            IssueId = dto.IssueId,
            Field = dto.Field,
            OldValue = dto.OldValue,
            NewValue = dto.NewValue,
            ChangedByUserId = dto.ChangedByUserId,
            ChangedAt = DateTime.UtcNow
        };

        await historyRepository.AddAsync(entry, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}