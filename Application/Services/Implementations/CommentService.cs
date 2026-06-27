using Domain.Entities;
using ProjectPlanner.Application.Common.Dto.Comment;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class CommentService(
    ICommentRepository commentRepository,
    IIssueRepository issueRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ICommentService
{
    // ---------------- CREATE ----------------

    public async Task<CommentDto> CreateAsync(
        int issueId,
        CreateCommentDto dto,
        CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(issueId, ct)
            ?? throw new KeyNotFoundException($"Issue {issueId} not found.");

        var author = await userRepository.GetByIdAsync(dto.AuthorId, ct)
            ?? throw new KeyNotFoundException($"User {dto.AuthorId} not found.");

        var comment = new IssueComment
        {
            IssueId = issue.Id,
            AuthorId = dto.AuthorId,
            Body = dto.Body,
            CreatedAt = DateTime.UtcNow,
            EditedAt = DateTime.UtcNow
        };

        await commentRepository.AddAsync(comment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return MapToDto(comment, author.Username);
    }

    // ---------------- GET BY ID ----------------

    public async Task<CommentDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var comment = await commentRepository.GetByIdAsync(id, ct);
        if (comment is null) return null;

        return MapToDto(comment, comment.Author?.Username);
    }

    // ---------------- GET BY ISSUE ----------------

    public async Task<IEnumerable<CommentDto>> GetByIssueIdAsync(int issueId, CancellationToken ct = default)
    {
        var issue = await issueRepository.GetByIdAsync(issueId, ct)
            ?? throw new KeyNotFoundException($"Issue {issueId} not found.");

        var comments = await commentRepository.GetByIssueIdAsync(issueId, ct);

        return comments.Select(c =>
            MapToDto(c, c.Author?.Username));
    }

    // ---------------- UPDATE ----------------

    public async Task<CommentDto> UpdateAsync(
        int id,
        UpdateCommentDto dto,
        CancellationToken ct = default)
    {
        var comment = await commentRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Comment {id} not found.");

        comment.Body = dto.Body;
        comment.EditedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(ct);

        return MapToDto(comment, comment.Author?.Username);
    }

    // ---------------- DELETE ----------------

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var comment = await commentRepository.GetByIdAsync(id, ct);
        if (comment is null) return false;

        commentRepository.Remove(comment);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    // ---------------- MAPPER ----------------

    private static CommentDto MapToDto(IssueComment comment, string? authorName)
    {
        return new CommentDto
        {
            Id = comment.Id,
            IssueId = comment.IssueId,
            Body = comment.Body,
            AuthorId = comment.AuthorId,
            AuthorName = authorName,
            CreatedAt = comment.CreatedAt,
            EditedAt = comment.EditedAt == comment.CreatedAt ? null : comment.EditedAt,
            Attachments = []
        };
    }
}