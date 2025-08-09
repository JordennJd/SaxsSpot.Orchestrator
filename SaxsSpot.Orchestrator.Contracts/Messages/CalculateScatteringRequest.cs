using System.Text.Json.Serialization;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Contracts.Messages;

/// <summary>
/// request for calculation worker
/// </summary>
/// <param name="RequestId"></param>
/// <param name="QVectorSpaceParameters"></param>
/// <param name="ThetaVectorSpaceParameters"></param>
/// <param name="PhiVectorSpaceParameters"></param>
/// <param name="SystemId"></param>
public record CalculateScatteringRequest(
    [property: JsonPropertyName("requestId")]
    string RequestId,

    [property: JsonPropertyName("qVectorSpaceParameters")]
    SpaceParameters QVectorSpaceParameters,

    [property: JsonPropertyName("thetaVectorSpaceParameters")]
    SpaceParameters ThetaVectorSpaceParameters,

    [property: JsonPropertyName("phiVectorSpaceParameters")]
    SpaceParameters PhiVectorSpaceParameters,

    [property: JsonPropertyName("systemId")]
    Guid SystemId
);
