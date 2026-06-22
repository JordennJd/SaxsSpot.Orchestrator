using FluentResults;
using MassTransit.Internals;
using MediatR;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChartAverage;

/// <summary>
/// Builds a single chart with averaged intensity over selected calculations.
/// Averaging is only applied when all calculations have the same number of points on the x-axis (Q),
/// and optionally when Q values align (same grid).
/// </summary>
public class PlotChartAverageHandler(
    ICalculationStorage storage,
    ICalculateObjectStorage objectStorage,
    IChartService chartService
) : IRequestHandler<PlotChartAverageRequest, IResult<string>>
{
    public async Task<IResult<string>> Handle(PlotChartAverageRequest request, CancellationToken cancellationToken)
    {
        var datasets = new List<(double[] X, double[] Y)>();

        var calculations = await storage.WhereAsync(x => request.CalculatesId.Contains(x.Id));
        foreach (var calculation in calculations)
        {
            var points = await objectStorage.Load(calculation.ObjectId, cancellationToken).ToListAsync(cancellationToken);
            if (points.Count == 0)
                continue;

            var x = points.Select(p => p.QVector).ToArray();
            var y = points.Select(p => p.Intensity).ToArray();
            datasets.Add((x, y));
        }

        if (datasets.Count == 0)
            return FluentResults.Result.Fail<string>("No data found in the selected calculations.");

        // Average strictly by index (first dataset defines the x-axis values).
        // We only require that all datasets have the same amount of points.
        var xRef = datasets[0].X;
        var n = xRef.Length;
        var count = datasets.Count;

        var ySum = new double[n];
        for (var i = 0; i < n; i++)
        {
            var sum = 0.0;
            for (var k = 0; k < count; k++)
                sum += datasets[k].Y[i];
            ySum[i] = sum / count;
        }

        var averageDataset = new Dataset
        {
            id = $"By model (n={count})",
            x = (double[])xRef.Clone(),
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
}
