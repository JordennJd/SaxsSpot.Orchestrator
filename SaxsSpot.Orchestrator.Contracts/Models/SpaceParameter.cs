using SaxsSpot.Orchestrator.Contracts.Enums;

namespace SaxsSpot.Orchestrator.Contracts.Models;

using System.Text.Json.Serialization;

public record SpaceParameters(
    [property: JsonPropertyName("spaceMethod")]
    SpaceMethod SpaceMethod,

    [property: JsonPropertyName("scaleMethod")]
    ScaleMethod ScaleMethod,

    [property: JsonPropertyName("spaceParameter")]
    float SpaceParameter,

    [property: JsonPropertyName("start")]
    float Start,

    [property: JsonPropertyName("end")]
    float End
);
