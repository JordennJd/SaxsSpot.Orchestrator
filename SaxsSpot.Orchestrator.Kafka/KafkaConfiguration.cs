using System.Text.Json.Serialization;

namespace SaxsSpot.Orchestrator.Kafka;

public record KafkaConfiguration
{
    [JsonPropertyName("brokers")]
    public List<string> Brokers { get; set; } = new();

    [JsonPropertyName("group")]
    public string Group { get; set; } = string.Empty;

    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;

    [JsonPropertyName("result-topic")]
    public string ResultTopic { get; set; } = string.Empty;
}