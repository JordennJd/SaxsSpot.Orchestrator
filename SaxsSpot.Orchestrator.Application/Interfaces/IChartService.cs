using SaxsSpot.Orchestrator.Contracts.Enums;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;

namespace SaxsSpot.Orchestrator.Application.Interfaces;

public interface IChartService
{
    Task<FluentResults.Result<string>> BuildChartAsync(
        string chartTitle,
        string xAxis,
        string yAxis,
        Dataset[] datasets,
        SpaceMethod scaleMethodsX,
        SpaceMethod scaleMethodsY,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns PNG as base64 string (no data URI prefix).
    /// </summary>
    Task<FluentResults.Result<string>> BuildChartPngAsync(
        string chartTitle,
        string xAxis,
        string yAxis,
        Dataset[] datasets,
        SpaceMethod scaleMethodsX,
        SpaceMethod scaleMethodsY,
        CancellationToken cancellationToken = default);
}

