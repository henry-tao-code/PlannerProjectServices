using Microsoft.Extensions.Options;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Infrastructure.AI.Configurations;
using ProjectPlanner.Infrastructure.AI.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProjectPlanner.Infrastructure.AI;

public sealed class LiteLlmService(
    HttpClient httpClient,
    IOptions<LiteLlmOptions> options
) : ILlmService
{
    private readonly LiteLlmOptions _options = options.Value;

    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Model))
        {
            throw new InvalidOperationException("LiteLLM model is not configured.");
        }

        var requestPayload = new
        {
            model = _options.Model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.2
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/v1/chat/completions")
        {
            Content = JsonContent.Create(requestPayload)
        };

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"LiteLLM API call failed with status code {response.StatusCode}. Response: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<LiteLlmResponseDto>(cancellationToken);

        return result?.Choices?.FirstOrDefault()?.Message?.Content
               ?? throw new InvalidOperationException("LiteLLM returned an empty response.");
    }
}