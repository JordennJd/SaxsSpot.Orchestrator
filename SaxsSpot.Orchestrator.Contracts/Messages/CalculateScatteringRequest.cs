using System.Text.Json.Serialization;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Contracts.Messages;

/// <summary>
/// Request for calculation worker
/// </summary>
public record CalculateScatteringRequest
{
    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }

    [JsonPropertyName("qVectorSpaceParameters")]
    public SpaceParameters QVectorSpaceParameters { get; init; }

    [JsonPropertyName("thetaVectorSpaceParameters")]
    public SpaceParameters ThetaVectorSpaceParameters { get; init; }

    [JsonPropertyName("phiVectorSpaceParameters")]
    public SpaceParameters PhiVectorSpaceParameters { get; init; }

    [JsonPropertyName("systemId")]
    public Guid SystemId { get; init; }
}