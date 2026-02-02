using System.Text;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Sdk.Interfaces;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.BuildChartAnalyse;

public class BuildChartAnalyseHandler(
    INanosystemServiceApiClient nanosystemServiceApiClient,
    IChartService chartService,
    ILogger<BuildChartAnalyseHandler> logger
) : IRequestHandler<BuildChartAnalyseRequest, IResult<string>>
{
    public async Task<IResult<string>> Handle(BuildChartAnalyseRequest request, CancellationToken cancellationToken)
    {
        if (request.RadialAnalysisIds == null || request.RadialAnalysisIds.Length == 0)
        {
            return FluentResults.Result.Fail<string>("At least one RadialAnalysisId is required");
        }

        try
        {
            var datasets = new List<Dataset>();

            foreach (var radialAnalysisId in request.RadialAnalysisIds)
            {
                await using var stream = await nanosystemServiceApiClient.DownloadRadialAnalysis(radialAnalysisId, cancellationToken);
                var dataPoints = await ParseDataFileAsync(stream, cancellationToken);

                if (dataPoints.Count == 0)
                {
                    logger.LogWarning("No data points in radial analysis {RadialAnalysisId}, skipping", radialAnalysisId);
                    continue;
                }

                datasets.Add(new Dataset
                {
                    id = radialAnalysisId.ToString(),
                    x = dataPoints.Select(p => p.Index).ToArray(),
                    y = dataPoints.Select(p => p.Value).ToArray()
                });
            }

            if (datasets.Count == 0)
            {
                return FluentResults.Result.Fail<string>("No valid data found in the selected radial analyses");
            }

            return await chartService.BuildChartAsync(
                request.ChartTitle,
                request.XAxis,
                request.YAxis,
                datasets.ToArray(),
                request.ScaleMethodsX,
                request.ScaleMethodsY,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to build chart for radial analyses: {Message}", ex.Message);
            return FluentResults.Result.Fail<string>($"Failed to build chart: {ex.Message}");
        }
    }

    private async Task<List<DataPoint>> ParseDataFileAsync(Stream stream, CancellationToken cancellationToken)
    {
        var dataPoints = new List<DataPoint>();
        
        using var reader = new StreamReader(stream, Encoding.UTF8);
        string? line;
        
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                if (double.TryParse(parts[0], out var index) && double.TryParse(parts[1], out var value))
                {
                    dataPoints.Add(new DataPoint { Index = index, Value = value });
                }
            }
        }

        return dataPoints;
    }

    private class DataPoint
    {
        public double Index { get; set; }
        public double Value { get; set; }
    }
}

