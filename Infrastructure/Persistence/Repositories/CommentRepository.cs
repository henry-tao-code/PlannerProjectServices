using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Infrastructure.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class CommentRepository(ProjectPlannerDbContext context) : ICommentRepository
{
    public async Task<IssueComment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Comments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<IssueComment>> GetByIssueIdAsync(int issueId, CancellationToken cancellationToken = default)
    {
        return await context.Comments
            .Include(c => c.Author)
            .Where(c => c.IssueId == issueId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(IssueComment comment, CancellationToken cancellationToken = default)
    {
        await context.Comments.AddAsync(comment, cancellationToken);
    }

    public void Update(IssueComment comment)
    {
        context.Comments.Update(comment);
    }

    public void Remove(IssueComment comment)
    {
        context.Comments.Remove(comment);
    }
}