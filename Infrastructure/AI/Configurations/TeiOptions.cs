namespace ProjectPlanner.Infrastructure.AI.Configurations;

public sealed class TeiOptions
{
    public string BaseUrl { get; set; } = "http://localhost:8080";
    public int TimeoutSeconds { get; set; } = 120;
    public int BatchSize { get; set; } = 16;
    public int MaxRetries { get; set; } = 3;
    public int RetryBaseDelayMilliseconds { get; set; } = 500;
}
