using Contracts.Dtos;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Infrastructure.AI.Configurations;
using ProjectPlanner.Infrastructure.AI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace ProjectPlanner.Infrastructure.AI;

public class TeiEmbeddingService(
    HttpClient httpClient,
    IOptions<TeiOptions> options,
    ILogger<TeiEmbeddingService> logger) : ITEIEmbeddingService
{
    private readonly TeiOptions _options = options.Value;

    public async Task<EmbeddingResponseDto> GetEmbeddingsAsync(
        EmbeddingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request?.Inputs == null || request.Inputs.Count == 0 || request.Inputs.All(string.IsNullOrWhiteSpace))
        {
            return new EmbeddingResponseDto { Embeddings = [] };
        }

        var allEmbeddings = new List<float[]>();
        var batchSize = Math.Clamp(_options.BatchSize, 1, 128);
        var batches = request.Inputs.Chunk(batchSize).ToArray();

        for (var batchIndex = 0; batchIndex < batches.Length; batchIndex++)
        {
            var batch = batches[batchIndex];
            var teiPayload = new TeiEmbedRequestDto
            {
                Inputs = [.. batch],
                Truncate = request.Truncate
            };

            using var response = await SendWithRetryAsync(
                teiPayload,
                batchIndex + 1,
                batches.Length,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);

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

    private async Task<HttpResponseMessage> SendWithRetryAsync(
        TeiEmbedRequestDto payload,
        int batchNumber,
        int batchCount,
        CancellationToken cancellationToken)
    {
        var maxAttempts = Math.Max(1, _options.MaxRetries + 1);

        for (var attempt = 1; ; attempt++)
        {
            var startedAt = DateTime.UtcNow;
            try
            {
                var response = await httpClient.PostAsJsonAsync("embed", payload, cancellationToken);
                var transientStatus = response.StatusCode is
                    System.Net.HttpStatusCode.RequestTimeout or
                    System.Net.HttpStatusCode.TooManyRequests ||
                    (int)response.StatusCode >= 500;

                if (!transientStatus || attempt >= maxAttempts)
                {
                    logger.LogInformation(
                        "TEI batch {BatchNumber}/{BatchCount} completed in {ElapsedMs} ms with HTTP {StatusCode}",
                        batchNumber, batchCount, (DateTime.UtcNow - startedAt).TotalMilliseconds,
                        (int)response.StatusCode);
                    return response;
                }

                response.Dispose();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (
                attempt < maxAttempts &&
                ex is HttpRequestException or TaskCanceledException)
            {
                logger.LogWarning(
                    ex,
                    "Transient TEI failure for batch {BatchNumber}/{BatchCount}, attempt {Attempt}/{MaxAttempts}",
                    batchNumber, batchCount, attempt, maxAttempts);
            }

            var exponentialDelay = _options.RetryBaseDelayMilliseconds * Math.Pow(2, attempt - 1);
            var jitter = Random.Shared.Next(0, 250);
            await Task.Delay(
                TimeSpan.FromMilliseconds(Math.Min(exponentialDelay + jitter, 10_000)),
                cancellationToken);
        }
    }
}
