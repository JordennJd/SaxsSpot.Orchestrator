using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Application.Services;
using SaxsSpot.Orchestrator.Contracts.Enums;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotScatteringCompare;

public record PlotScatteringCompareRequest(
    string ChartTitle,
    string XAxis,
    string YAxis,
    List<Guid> LegacyCalculationIds,
    List<Guid> NanoScatteringIds,
    bool AverageLegacy,
    bool AverageNano,
    SpaceMethod ScaleMethodsX,
    SpaceMethod ScaleMethodsY) : IRequest<IResult<string>>;

public class PlotScatteringCompareHandler(
    ScatteringCompareChartBuilder compareChartBuilder,
    IChartService chartService) : IRequestHandler<PlotScatteringCompareRequest, IResult<string>>
{
    public async Task<IResult<string>> Handle(PlotScatteringCompareRequest request, CancellationToken cancellationToken)
    {
        var chartDatasets = await compareChartBuilder.BuildDatasetsAsync(
            request.LegacyCalculationIds,
            request.NanoScatteringIds,
            request.AverageLegacy,
            request.AverageNano,
            cancellationToken);

        if (chartDatasets.Count == 0)
        {
            return FluentResults.Result.Fail<string>("No data found for comparison.");
        }

        return await chartService.BuildChartAsync(
            request.ChartTitle,
            request.XAxis,
            request.YAxis,
            chartDatasets.ToArray(),
            request.ScaleMethodsX,
            request.ScaleMethodsY,
            cancellationToken);
    }
}
