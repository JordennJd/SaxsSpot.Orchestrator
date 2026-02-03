using System.Text;
using System.Text.Json;
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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<string>> Handle(BuildChartAnalyseAverageRequest request, CancellationToken cancellationToken)
    {
        if (request.RadialAnalysisIds == null || request.RadialAnalysisIds.Length == 0)
        {
            return FluentResults.Result.Fail<string>("At least one RadialAnalysisId is required");
        }

        try
        {
            var allLayers = new List<List<RadialAnalysisLayerDto>>();

            foreach (var radialAnalysisId in request.RadialAnalysisIds)
            {
                await using var stream = await nanosystemServiceApiClient.DownloadRadialAnalysis(radialAnalysisId, cancellationToken);
                var layers = await ParseRadialAnalysisFileAsync(stream, cancellationToken);

                if (layers.Count == 0)
                {
                    logger.LogWarning("No layer data in radial analysis {RadialAnalysisId}, skipping", radialAnalysisId);
                    continue;
                }

                allLayers.Add(layers);
            }

            if (allLayers.Count == 0)
            {
                return FluentResults.Result.Fail<string>("No valid data found in the selected radial analyses");
            }

            var minLength = allLayers.Min(l => l.Count);
            if (minLength == 0)
            {
                return FluentResults.Result.Fail<string>("No data points to average");
            }

            var firstLayers = allLayers[0].Take(minLength).ToList();
            var xMidpoints = firstLayers.Select(l => l.Midpoint).ToArray();
            var xLabels = firstLayers.Select(l => l.AxisLabel).ToArray();
            var ySum = new double[minLength];
            var analysisCount = allLayers.Count;

            for (var i = 0; i < minLength; i++)
            {
                var sum = 0.0;
                for (var k = 0; k < analysisCount; k++)
                {
                    sum += allLayers[k][i].NumericalConcentration;
                }
                ySum[i] = sum / analysisCount;
            }

            var averageDataset = new Dataset
            {
                id = "average",
                x = xMidpoints,
                y = ySum,
                xLabels = xLabels
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

    private async Task<List<RadialAnalysisLayerDto>> ParseRadialAnalysisFileAsync(Stream stream, CancellationToken cancellationToken)
    {
        var layers = new List<RadialAnalysisLayerDto>();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        string? line;
        var isFirstLine = true;

        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            try
            {
                var layer = JsonSerializer.Deserialize<RadialAnalysisLayerDto>(line, JsonOptions);
                if (layer != null)
                    layers.Add(layer);
            }
            catch (JsonException)
            {
                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && double.TryParse(parts[0], out var index) && double.TryParse(parts[1], out var value))
                {
                    layers.Add(new RadialAnalysisLayerDto
                    {
                        LayerIndex = (int)index,
                        LayerFrom = index,
                        LayerTo = index,
                        NumericalConcentration = value,
                        PointCount = 0
                    });
                }
            }
        }

        return layers.OrderBy(l => l.LayerIndex).ToList();
    }
}
