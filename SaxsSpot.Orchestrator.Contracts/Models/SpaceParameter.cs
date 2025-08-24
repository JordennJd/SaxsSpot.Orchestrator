using SaxsSpot.Orchestrator.Contracts.Enums;

namespace SaxsSpot.Orchestrator.Contracts.Models;

using System.Text.Json.Serialization;

public record SpaceParameters(
    [property: JsonPropertyName("spaceMethod")]
    SpaceMethod SpaceMethod,

    [property: JsonPropertyName("scaleMethod")]
    ScaleMethod ScaleMethod,

    [property: JsonPropertyName("spaceParameter")]
    double SpaceParameter,

    [property: JsonPropertyName("start")]
    double Start,

    [property: JsonPropertyName("end")]
    double End
);
