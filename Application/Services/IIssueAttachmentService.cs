using Contracts.Events;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using ProjectPlanner.Application.Common.Dtos.Issue;

namespace ProjectPlanner.Application.Services;

public interface IIssueAttachmentService
{
    Task<IssueAttachmentDto> UploadAsync(
        int issueId,
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<IssueAttachment>> GetByIssueIdAsync(
        int issueId,
        CancellationToken cancellationToken = default);

    Task<IssueAttachment?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task MarkProcessedAsync(
        DocumentProcessedEvent message,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long id,
        CancellationToken cancellationToken = default);
}