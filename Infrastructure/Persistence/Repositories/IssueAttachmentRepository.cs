using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class IssueAttachmentRepository(ProjectPlannerDbContext dbContext)
    : IIssueAttachmentRepository
{
    public async Task<IssueAttachment?> GetByIdAsync(
        long id,
        CancellationToken ct = default)
    {
        return await dbContext.IssueAttachments
            .Include(a => a.UploadedBy)
            .Include(a => a.DeletedBy)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<List<IssueAttachment>> GetByIssueIdAsync(
        int issueId,
        CancellationToken ct = default)
    {
        return await dbContext.IssueAttachments
            .Where(a =>
                a.IssueId == issueId &&
                !a.IsDeleted)
            .Include(a => a.UploadedBy)
            .OrderByDescending(a => a.UploadedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(
        IssueAttachment attachment,
        CancellationToken ct = default)
    {
        await dbContext.IssueAttachments.AddAsync(attachment, ct);
    }

    public void Update(IssueAttachment attachment)
    {
        dbContext.IssueAttachments.Update(attachment);
    }

    public void Remove(IssueAttachment attachment)
    {
        dbContext.IssueAttachments.Remove(attachment);
    }
}