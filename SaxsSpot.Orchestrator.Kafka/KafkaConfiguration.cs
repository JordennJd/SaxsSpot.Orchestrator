using Microsoft.Extensions.Configuration;

namespace SaxsSpot.Orchestrator.Kafka;

public record KafkaConfiguration
{
    [ConfigurationKeyName("brokers")]
    public List<string> Brokers { get; set; } = new();

    [ConfigurationKeyName("group")]
    public string Group { get; set; } = string.Empty;

    [ConfigurationKeyName("result-topic")]
    public string ResultTopic { get; set; } = string.Empty;
    
    [ConfigurationKeyName("topic")]
    public string Topic { get; set; } = string.Empty;
    
    [ConfigurationKeyName("run-generation-topic")]
    public string RunGenerationTopic { get; set; } = string.Empty;

}