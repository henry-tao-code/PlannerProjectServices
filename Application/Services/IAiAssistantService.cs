namespace ProjectPlanner.Application.Services;

public interface IAiAssistantService
{
    Task<string> GenerateAcceptanceCriteriaAsync(string issueTitle, string issueDescription, CancellationToken ct = default);
}
