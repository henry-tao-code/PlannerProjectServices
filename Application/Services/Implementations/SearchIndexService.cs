using Domain.Entities;
using Domain.Enums;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class SearchIndexService(
    IIssueRepository issueRepository,
    IEpicRepository epicRepository,
    ISearchIndexRepository searchRepository,
    IUnitOfWork unitOfWork) : ISearchIndexService
{
    public async Task IndexIssueAsync(
        int issueId,
        CancellationToken ct)
    {
        var issue = await issueRepository.GetByIdAsync(issueId, ct) ?? throw new KeyNotFoundException(
                $"Issue {issueId} not found.");
        var document = await searchRepository.GetAsync(
            SearchEntityType.Issue,
            issueId,
            ct);

        if (document is null)
        {
            document = new SearchDocument
            {
                EntityType = SearchEntityType.Issue,
                EntityId = issueId
            };
            await searchRepository.AddAsync(document, ct);
        }

        document.ProjectId = issue.ProjectId;
        document.Title = issue.Title;
        document.Content = issue.Description ?? string.Empty;
        document.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task IndexEpicAsync(
        int epicId,
        CancellationToken ct)
    {
        var epic = await epicRepository.GetByIdAsync(epicId, ct) ?? throw new KeyNotFoundException(
                $"Epic {epicId} not found.");
        var document = await searchRepository.GetAsync(
            SearchEntityType.Epic,
            epicId,
            ct);

        if (document is null)
        {
            document = new SearchDocument
            {
                EntityType = SearchEntityType.Epic,
                EntityId = epicId
            };
            await searchRepository.AddAsync(document, ct);
        }

        document.ProjectId = epic.ProjectId;
        document.Title = epic.Title;
        document.Content = epic.Description ?? string.Empty;
        document.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(
        SearchEntityType entityType,
        int entityId,
        CancellationToken ct = default)
    {
        var document = await searchRepository.GetAsync(
            entityType,
            entityId,
            ct);

        if (document is null)
            return;

        await searchRepository.DeleteAsync(document, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }
}