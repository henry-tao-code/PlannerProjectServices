namespace ProjectPlanner.Infrastructure.AI.Configurations;

public class LiteLlmOptions
{
    public const string SectionName = "LiteLLM";

    public string BaseUrl { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}