using ProjectPlanner.Application.Common.Dtos.Issue;

namespace ProjectPlanner.Application.Services;

public interface IIssueService
{
    // ---------------- READ ----------------
    Task<IssueDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<IssueSummaryDto>> GetByProjectAsync(int projectId, CancellationToken ct = default);
    Task<IEnumerable<IssueSummaryDto>> SearchIssuesAsync(int projectId, string searchTerm, CancellationToken ct = default);

    // ---------------- CREATE ----------------
    Task<IssueDetailDto> CreateAsync(IssueCreateDto dto, CancellationToken ct = default);

    // ---------------- UPDATE (split by intent) ----------------
    Task<IssueDetailDto> UpdateCoreAsync(int id, UpdateIssueCoreDto dto, CancellationToken ct = default);

    Task<IssueDetailDto> UpdateAssignmentAsync(int id, UpdateIssueAssignmentDto dto, CancellationToken ct = default);

    Task<IssueDetailDto> MoveIssueAsync(int issueId, MoveIssueDto dto, CancellationToken ct = default);

    Task<IssueDetailDto> UpdateTimeTrackingAsync(int id, UpdateIssueTimeTrackingDto dto, CancellationToken ct = default);

    Task<IssueDetailDto> UpdateLabelsAsync(int id, UpdateIssueLabelsDto dto, CancellationToken ct = default);

    // ---------------- DELETE ----------------
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}