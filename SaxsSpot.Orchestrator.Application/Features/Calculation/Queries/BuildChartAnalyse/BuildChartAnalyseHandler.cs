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
        try
        {
            // Получаем файл с данными анализа
            await using var stream = await nanosystemServiceApiClient.DownloadRadialAnalysis(request.RadialAnalysisId, cancellationToken);
            
            // Парсим файл в формате "индекс значение"
            var dataPoints = await ParseDataFileAsync(stream, cancellationToken);
            
            if (dataPoints.Count == 0)
            {
                return FluentResults.Result.Fail<string>("No data points found in the radial analysis file");
            }

            // Создаем dataset для графика
            var dataset = new Dataset
            {
                id = request.RadialAnalysisId.ToString(),
                x = dataPoints.Select(p => p.Index).ToArray(),
                y = dataPoints.Select(p => p.Value).ToArray()
            };

            // Строим график используя общий сервис
            return await chartService.BuildChartAsync(
                request.ChartTitle,
                request.XAxis,
                request.YAxis,
                new[] { dataset },
                request.ScaleMethodsX,
                request.ScaleMethodsY,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to build chart for radial analysis {RadialAnalysisId}: {Message}", 
                request.RadialAnalysisId, ex.Message);
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

