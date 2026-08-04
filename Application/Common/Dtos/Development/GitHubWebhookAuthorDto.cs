namespace ProjectPlanner.Application.Common.Dtos.Development;

public record GitHubWebhookAuthorDto
{
    public string Name { get; init; } = string.Empty;
}
