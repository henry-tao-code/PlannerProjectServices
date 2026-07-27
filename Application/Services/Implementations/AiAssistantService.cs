using Microsoft.Extensions.AI;

namespace ProjectPlanner.Application.Services.Implementations;

public class AiAssistantService(IChatClient chatClient)
{
    public async Task<string> GenerateAcceptanceCriteriaAsync(string issueTitle, string issueDescription, CancellationToken ct = default)
    {
        var prompt = $"""
            You are an agile product owner assistant.
            Generate 3-5 concise acceptance criteria for the following issue:
            
            Title: {issueTitle}
            Description: {issueDescription}
            """;

        // Dispatch prompt to LiteLLM Gateway
        var response = await chatClient.GetResponseAsync(prompt, cancellationToken: ct);

        return response.Text;
    }
}