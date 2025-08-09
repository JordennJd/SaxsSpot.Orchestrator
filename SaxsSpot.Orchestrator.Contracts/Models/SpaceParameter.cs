namespace SaxsSpot.Orchestrator.Contracts.Models;

using System.Text.Json.Serialization;

public record SpaceParameters(
    [property: JsonPropertyName("spaceMethod")]
    SpaceMethods SpaceMethod,

    [property: JsonPropertyName("scaleMethod")]
    ScaleMethods ScaleMethod,

    [property: JsonPropertyName("spaceParameter")]
    float SpaceParameter,

    [property: JsonPropertyName("start")]
    float Start,

    [property: JsonPropertyName("end")]
    float End
);
