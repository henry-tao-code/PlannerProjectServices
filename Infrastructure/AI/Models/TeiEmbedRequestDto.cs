using System.Text.Json.Serialization;

namespace ProjectPlanner.Infrastructure.AI.Models;

internal sealed class TeiEmbedRequestDto
{
    [JsonPropertyName("inputs")]
    public List<string> Inputs { get; set; } = [];

    [JsonPropertyName("truncate")]
    public bool Truncate { get; set; } = true;
}