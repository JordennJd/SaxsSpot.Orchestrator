using System.Net.Http.Json;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.Enums;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;

namespace SaxsSpot.Orchestrator.Application.Services;

public class ChartService(IConfiguration configuration, ILogger<ChartService> logger) : IChartService
{
    public async Task<Result<string>> BuildChartAsync(
        string chartTitle,
        string xAxis,
        string yAxis,
        Dataset[] datasets,
        SpaceMethod scaleMethodsX,
        SpaceMethod scaleMethodsY,
        CancellationToken cancellationToken = default)
    {
        var plotRequest = new PlotRequest
        {
            title = chartTitle,
            x_label = xAxis,
            y_label = yAxis,
            x_log_scale = scaleMethodsX == SpaceMethod.Log,
            y_log_scale = scaleMethodsY == SpaceMethod.Log,
            datasets = datasets
        };

        foreach (var dataset in datasets)
        {
            dataset.SortByX();
        }

        using var client = new HttpClient();

        try
        {
            var chartUri = configuration.GetValue<string>("chart:uri");
            if (string.IsNullOrEmpty(chartUri))
            {
                return FluentResults.Result.Fail<string>("Chart URI is not configured");
            }

            var response = await client.PostAsJsonAsync($"{chartUri}/plot", plotRequest, cancellationToken);
            response.EnsureSuccessStatusCode();
            var htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return FluentResults.Result.Ok(htmlContent);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to build chart: {Message}", e.Message);
            return FluentResults.Result.Fail<string>("Build chart failed");
        }
    }

    public async Task<Result<string>> BuildChartPngAsync(
        string chartTitle,
        string xAxis,
        string yAxis,
        Dataset[] datasets,
        SpaceMethod scaleMethodsX,
        SpaceMethod scaleMethodsY,
        CancellationToken cancellationToken = default)
    {
        var plotRequest = new PlotRequest
        {
            title = chartTitle,
            x_label = xAxis,
            y_label = yAxis,
            x_log_scale = scaleMethodsX == SpaceMethod.Log,
            y_log_scale = scaleMethodsY == SpaceMethod.Log,
            datasets = datasets
        };

        foreach (var dataset in datasets)
        {
            dataset.SortByX();
        }

        using var client = new HttpClient();

        try
        {
            var chartUri = configuration.GetValue<string>("chart:uri");
            if (string.IsNullOrEmpty(chartUri))
            {
                return FluentResults.Result.Fail<string>("Chart URI is not configured");
            }

            using var response = await client.PostAsJsonAsync($"{chartUri}/plot/png", plotRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                var status = (int)response.StatusCode;
                logger.LogError("Chart PNG request failed: {Status} {Body}", status, body);
                return FluentResults.Result.Fail<string>($"Build chart PNG failed: HTTP {status}. {body}");
            }

            var pngBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var base64 = Convert.ToBase64String(pngBytes);
            return FluentResults.Result.Ok(base64);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to build chart PNG: {Message}", e.Message);
            return FluentResults.Result.Fail<string>($"Build chart PNG failed: {e.Message}");
        }
    }
}

