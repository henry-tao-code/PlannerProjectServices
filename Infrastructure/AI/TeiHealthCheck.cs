using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ProjectPlanner.Infrastructure.AI;

public sealed class TeiHealthCheck(IHttpClientFactory httpClientFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = httpClientFactory.CreateClient("TeiHealth");
            using var response = await client.GetAsync("health", cancellationToken);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("TEI is ready.")
                : HealthCheckResult.Unhealthy($"TEI returned HTTP {(int)response.StatusCode}.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return HealthCheckResult.Unhealthy("TEI is unreachable or timed out.", ex);
        }
    }
}
