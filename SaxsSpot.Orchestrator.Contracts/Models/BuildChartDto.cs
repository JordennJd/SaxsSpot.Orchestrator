using SaxsSpot.Orchestrator.Contracts.Enums;

namespace SaxsSpot.Orchestrator.Contracts.Models;

public record BuildChartDto
(
    string ChartTitle,
    string XName,
    string YName,
    List<string> CalculatesId,
    ScaleMethod ScaleMethodsX,
    ScaleMethod ScaleMethodsY
);