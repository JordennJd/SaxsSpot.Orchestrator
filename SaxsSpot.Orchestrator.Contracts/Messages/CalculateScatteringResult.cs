namespace SaxsSpot.Orchestrator.Contracts.Messages;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Result of saxs calculation
/// </summary>
public record ScatteringResult
{
    [JsonPropertyName("request")]
    public CalculateScatteringRequest Request { get; set; } = default!;

    [JsonPropertyName("result")]
    public List<IntensityResult> Result { get; set; } = new();
}

public record IntensityResult
{
    [JsonPropertyName("qVector")]
    public float QVector { get; set; }

    [JsonPropertyName("intensity")]
    public float Intensity { get; set; }
}
