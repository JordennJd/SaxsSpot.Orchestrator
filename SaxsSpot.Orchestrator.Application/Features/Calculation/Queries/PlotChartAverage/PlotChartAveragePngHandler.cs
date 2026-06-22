using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChartAveragePng;

public class PlotChartAveragePngHandler(
    ICalculationStorage storage,
    ICalculateObjectStorage objectStorage,
    IChartService chartService
) : IRequestHandler<PlotChartAveragePngRequest, IResult<string>>
{
    public async Task<IResult<string>> Handle(
        PlotChartAveragePngRequest request,
        CancellationToken cancellationToken)
    {
        var datasets = new List<(double[] X, double[] Y)>();

        var calculations = await storage.WhereAsync(x => request.CalculatesId.Contains(x.Id));
        foreach (var calculation in calculations)
        {
            var points = new List<IntensityResult>();
            await foreach (var p in objectStorage.Load(calculation.ObjectId, cancellationToken)
                               .WithCancellation(cancellationToken))
            {
                points.Add(p);
            }

            if (points.Count == 0)
                continue;

            datasets.Add((points.Select(p => p.QVector).ToArray(), points.Select(p => p.Intensity).ToArray()));
        }

        if (datasets.Count == 0)
            return FluentResults.Result.Fail<string>("No data found in the selected calculations.");

        var n = datasets[0].X.Length;
        foreach (var (x, _) in datasets)
        {
            if (x.Length != n)
                return FluentResults.Result.Fail<string>(
                    "Cannot average: abscissa count does not match across calculations. " +
                    $"Expected {n} points everywhere. Select calculations with the same Q grid (same number of points).");
        }

        var xRef = datasets[0].X;
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

        return await chartService.BuildChartPngAsync(
            request.ChartTitle,
            request.XAxis,
            request.YAxis,
            new[] { averageDataset },
            request.ScaleMethodsX,
            request.ScaleMethodsY,
            cancellationToken);
    }
}

