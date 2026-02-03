using System.Text.Json.Serialization;

namespace SaxsSpot.Orchestrator.Contracts.ExternalDto;

/// <summary>
/// Layer row from radial analysis (matches saxs.radial_analysis_layer).
/// Used when parsing downloaded radial analysis file (JSON lines).
/// </summary>
public class RadialAnalysisLayerDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("radialAnalysisId")]
    public Guid RadialAnalysisId { get; set; }

    [JsonPropertyName("nanosystemId")]
    public Guid NanosystemId { get; set; }

    [JsonPropertyName("layerIndex")]
    public int LayerIndex { get; set; }

    [JsonPropertyName("layerFrom")]
    public double LayerFrom { get; set; }

    [JsonPropertyName("layerTo")]
    public double LayerTo { get; set; }

    [JsonPropertyName("numericalConcentration")]
    public double NumericalConcentration { get; set; }

    [JsonPropertyName("pointCount")]
    public int PointCount { get; set; }

    /// <summary>Midpoint radius for chart x-axis.</summary>
    public double Midpoint => (LayerFrom + LayerTo) / 2;

    /// <summary>Label for x-axis: layer index and radius range; point count only when &gt; 0.</summary>
    public string AxisLabel => PointCount > 0
        ? $"L{LayerIndex} [{LayerFrom:F2}-{LayerTo:F2}] n={PointCount}"
        : $"L{LayerIndex} [{LayerFrom:F2}-{LayerTo:F2}]";
}
