using Domain.Entities;
using Domain.Enums;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class SearchIndexService(
    IIssueRepository issueRepository,
    IEpicRepository epicRepository,
    IProjectRepository projectRepository,
    ISearchIndexRepository searchRepository) : ISearchIndexService
{
    public async Task IndexIssueAsync(
        int issueId,
        CancellationToken ct)
    {
        var issue = await issueRepository.GetByIdAsync(issueId, ct) ?? throw new KeyNotFoundException(
                $"Issue {issueId} not found.");
        var project = await projectRepository.GetByIdAsync(issue.ProjectId, ct)
            ?? throw new KeyNotFoundException($"Project {issue.ProjectId} not found.");

        await searchRepository.IndexDocumentAsync(new SearchDocument
        {
            EntityType = SearchEntityType.Issue.ToString(),
            EntityId = issue.Id,
            IssueKey = issue.IssueKey,
            Title = issue.Title,
            Description = issue.Description,
            ProjectId = issue.ProjectId,
            ProjectName = project.Name,
            Status = issue.Status.ToString(),
            Priority = issue.Priority.ToString(),
            AssigneeName = issue.Assignee?.Username,
            ReporterName = issue.Reporter?.Username,
            SprintId = issue.SprintId,
            SprintName = issue.Sprint?.Name
        }, ct);
    }

    public async Task IndexEpicAsync(
        int epicId,
        CancellationToken ct)
    {
        var epic = await epicRepository.GetByIdAsync(epicId, ct) ?? throw new KeyNotFoundException(
                $"Epic {epicId} not found.");
        var project = await projectRepository.GetByIdAsync(epic.ProjectId, ct)
            ?? throw new KeyNotFoundException($"Project {epic.ProjectId} not found.");

        await searchRepository.IndexDocumentAsync(new SearchDocument
        {
            EntityType = SearchEntityType.Epic.ToString(),
            EntityId = epic.Id,
            Title = epic.Title,
            Description = string.Join(" ", new[] { epic.Summary, epic.Description }
                .Where(value => !string.IsNullOrWhiteSpace(value))),
            ProjectId = epic.ProjectId,
            ProjectName = project.Name,
            Status = epic.Status.ToString(),
            AssigneeName = epic.Assignee?.Username
        }, ct);
    }

    public async Task RemoveAsync(
        SearchEntityType entityType,
        int entityId,
        CancellationToken ct = default)
    {
        await searchRepository.DeleteDocumentAsync(entityType.ToString(), entityId, ct);
    }
}
