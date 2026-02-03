using System.Text;
using System.Text.RegularExpressions;
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
    /// Parses radial analysis file: one line per layer "index (layerFrom - layerTo) NC [pointCount]".
    /// </summary>
    private static readonly Regex LineFormat = new(@"^\s*(\d+)\s*\(([\d.eE+-]+)\s*-\s*([\d.eE+-]+)\)\s*([\d.eE+-]+)(?:\s+(\d+))?\s*$");

    private async Task<List<RadialAnalysisLayerDto>> ParseRadialAnalysisFileAsync(Stream stream, CancellationToken cancellationToken)
    {
        var layers = new List<RadialAnalysisLayerDto>();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        string? line;

        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var m = LineFormat.Match(line);
            if (m.Success && int.TryParse(m.Groups[1].Value, out var index)
                && double.TryParse(m.Groups[2].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var from)
                && double.TryParse(m.Groups[3].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var to)
                && double.TryParse(m.Groups[4].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var nc))
            {
                var pointCount = m.Groups[5].Success && int.TryParse(m.Groups[5].Value, out var n) ? n : 0;
                layers.Add(new RadialAnalysisLayerDto
                {
                    LayerIndex = index,
                    LayerFrom = from,
                    LayerTo = to,
                    NumericalConcentration = nc,
                    PointCount = pointCount
                });
            }
        }

        return layers.OrderBy(l => l.LayerIndex).ToList();
    }
}

