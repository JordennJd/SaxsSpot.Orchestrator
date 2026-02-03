using System.Text;
using System.Text.Json;
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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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
                var layers = await ParseRadialAnalysisFileAsync(stream, cancellationToken);

                if (layers.Count == 0)
                {
                    logger.LogWarning("No layer data in radial analysis {RadialAnalysisId}, skipping", radialAnalysisId);
                    continue;
                }

                datasets.Add(new Dataset
                {
                    id = radialAnalysisId.ToString(),
                    x = layers.Select(l => l.Midpoint).ToArray(),
                    y = layers.Select(l => l.NumericalConcentration).ToArray(),
                    xLabels = layers.Select(l => l.AxisLabel).ToArray()
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

    /// <summary>
    /// Parses radial analysis file: first line = metadata JSON, then one JSON line per layer (id, nanosystemId, layerIndex, layerFrom, layerTo, numericalConcentration, pointCount).
    /// </summary>
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
                // Fallback: try legacy format "index value" (space-separated)
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

