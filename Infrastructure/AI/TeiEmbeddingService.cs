using Contracts.Dtos;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Infrastructure.AI.Models;
using System.Net.Http.Json;

namespace ProjectPlanner.Infrastructure.AI;

public class TeiEmbeddingService(HttpClient httpClient) : ITEIEmbeddingService
{
    public async Task<EmbeddingResponseDto> GetEmbeddingsAsync(
        EmbeddingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var teiPayload = new TeiEmbedRequest
        {
            Inputs = [.. request.Inputs],
            Truncate = request.Truncate
        };

        var response = await httpClient.PostAsJsonAsync("embed", teiPayload, cancellationToken);
        response.EnsureSuccessStatusCode();

        var vectors = await response.Content.ReadFromJsonAsync<List<float[]>>(cancellationToken: cancellationToken);

        return new EmbeddingResponseDto
        {
            Embeddings = vectors ?? []
        };
    }
}
