using ProjectPlanner.Application.Common.Dto.Comment;
using ProjectPlanner.Application.Common.Dtos.Comment;

namespace ProjectPlanner.Application.Services;

public interface ICommentService
{
    Task<CommentDto> CreateAsync(int issueId, CreateCommentDto dto, CancellationToken cancellationToken = default);
    Task<CommentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CommentDto>> GetByIssueIdAsync(int issueId, CancellationToken cancellationToken = default);
    Task<CommentDto> UpdateAsync(int id, UpdateCommentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}