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
}

