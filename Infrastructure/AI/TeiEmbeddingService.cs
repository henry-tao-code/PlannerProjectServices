using Contracts.Dtos;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Infrastructure.AI.Models;
using System.Net.Http.Json;

namespace ProjectPlanner.Infrastructure.AI;

public class TeiEmbeddingService(
    HttpClient httpClient
    // ILogger<TeiEmbeddingService> logger
    ) : ITEIEmbeddingService
{
    private const int MaxBatchSize = 32;

    public async Task<EmbeddingResponseDto> GetEmbeddingsAsync(
        EmbeddingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request?.Inputs == null || request.Inputs.Count == 0 || request.Inputs.All(string.IsNullOrWhiteSpace))
        {
            return new EmbeddingResponseDto { Embeddings = [] };
        }

        var allEmbeddings = new List<float[]>();
        var batches = request.Inputs.Chunk(MaxBatchSize);

        foreach (var batch in batches)
        {
            var teiPayload = new TeiEmbedRequestDto
            {
                Inputs = [.. batch],
                Truncate = request.Truncate
            };

            var response = await httpClient.PostAsJsonAsync("embed", teiPayload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);

                // logger.LogError(
                //     "TEI embedding request failed with status code {StatusCode}. Details: {ErrorDetails}",
                //     response.StatusCode,
                //     errorBody);

                throw new HttpRequestException(
                    $"TEI HTTP {(int)response.StatusCode} ({response.StatusCode}): {errorBody}",
                    inner: null,
                    statusCode: response.StatusCode);
            }

            var vectors = await response.Content.ReadFromJsonAsync<List<float[]>>(cancellationToken: cancellationToken);

            if (vectors != null)
            {
                allEmbeddings.AddRange(vectors);
            }
        }

        return new EmbeddingResponseDto
        {
            Embeddings = allEmbeddings
        };
    }
}