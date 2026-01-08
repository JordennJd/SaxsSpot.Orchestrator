using System.Text.Json.Serialization;

namespace SaxsSpot.Orchestrator.Contracts.Messages;

/// <summary>
/// Request for nanosystem generation
/// </summary>
public record RunGenerationRequest
{
    [JsonPropertyName("operationId")]
    public Guid OperationId { get; set; }
    
    [JsonPropertyName("parameters")]
    public CommonParticleGenerationParametersDto Parameters { get; init; } = default!;
}

/// <summary>
/// Common particle generation parameters DTO
/// </summary>
public record CommonParticleGenerationParametersDto
{
    [JsonPropertyName("count")]
    public int Count { get; init; }
    
    [JsonPropertyName("numericalConcentration")]
    public double? NumericalConcentration { get; init; }
    
    [JsonPropertyName("globalSize")]
    public double? GlobalSize { get; init; }
    
    [JsonPropertyName("minSize")]
    public float MinSize { get; init; }
    
    [JsonPropertyName("maxSize")]
    public float MaxSize { get; init; }
    
    [JsonPropertyName("theta")]
    public float Theta { get; init; }
    
    [JsonPropertyName("k")]
    public float K { get; init; }
    
    [JsonPropertyName("excess")]
    public double Excess { get; init; }
    
    [JsonPropertyName("epsilon")]
    public float? Epsilon { get; init; }
}
