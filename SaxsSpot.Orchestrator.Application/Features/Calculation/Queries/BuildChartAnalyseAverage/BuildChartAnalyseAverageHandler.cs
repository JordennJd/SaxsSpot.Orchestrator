using System.Text;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Sdk.Interfaces;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.BuildChartAnalyseAverage;

public class BuildChartAnalyseAverageHandler(
    INanosystemServiceApiClient nanosystemServiceApiClient,
    IChartService chartService,
    ILogger<BuildChartAnalyseAverageHandler> logger
) : IRequestHandler<BuildChartAnalyseAverageRequest, IResult<string>>
{
    public async Task<IResult<string>> Handle(BuildChartAnalyseAverageRequest request, CancellationToken cancellationToken)
    {
        if (request.RadialAnalysisIds == null || request.RadialAnalysisIds.Length == 0)
        {
            return FluentResults.Result.Fail<string>("At least one RadialAnalysisId is required");
        }

        try
        {
            var allDataPoints = new List<List<DataPoint>>();

            foreach (var radialAnalysisId in request.RadialAnalysisIds)
            {
                await using var stream = await nanosystemServiceApiClient.DownloadRadialAnalysis(radialAnalysisId, cancellationToken);
                var dataPoints = await ParseDataFileAsync(stream, cancellationToken);

                if (dataPoints.Count == 0)
                {
                    logger.LogWarning("No data points in radial analysis {RadialAnalysisId}, skipping", radialAnalysisId);
                    continue;
                }

                allDataPoints.Add(dataPoints);
            }

            if (allDataPoints.Count == 0)
            {
                return FluentResults.Result.Fail<string>("No valid data found in the selected radial analyses");
            }

            // Use minimum length so every analysis has a value at each index
            var minLength = allDataPoints.Min(dp => dp.Count);
            if (minLength == 0)
            {
                return FluentResults.Result.Fail<string>("No data points to average");
            }

            var firstX = allDataPoints[0].Take(minLength).Select(p => p.Index).ToArray();
            var ySum = new double[minLength];
            var analysisCount = allDataPoints.Count;

            for (var i = 0; i < minLength; i++)
            {
                var sum = 0.0;
                for (var k = 0; k < analysisCount; k++)
                {
                    sum += allDataPoints[k][i].Value;
                }
                ySum[i] = sum / analysisCount;
            }

            var averageDataset = new Dataset
            {
                id = "average",
                x = firstX,
                y = ySum
            };

            return await chartService.BuildChartAsync(
                request.ChartTitle,
                request.XAxis,
                request.YAxis,
                new[] { averageDataset },
                request.ScaleMethodsX,
                request.ScaleMethodsY,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to build average chart for radial analyses: {Message}", ex.Message);
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
