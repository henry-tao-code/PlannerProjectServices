using Microsoft.Extensions.Configuration;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Infrastructure.AI.Models;
using System.Net.Http.Json;

namespace ProjectPlanner.Infrastructure.AI;

public sealed class LiteLlmService(
        HttpClient httpClient,
        IConfiguration configuration
    ) : ILlmService
{

    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var model =
            configuration["LiteLLM:Model"]
            ?? throw new InvalidOperationException(
                "LiteLLM model is not configured.");

        var request = new
        {
            model,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            temperature = 0.2
        };

        using var response =
            await httpClient.PostAsJsonAsync(
                "/v1/chat/completions",
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<
                LiteLlmResponseDto>(
                    cancellationToken);

        return result?.Choices?.FirstOrDefault()?.Message?.Content
               ?? throw new InvalidOperationException(
                   "LiteLLM returned an empty response.");
    }
}