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
    // When Q grids are generated with the same input parameters, they usually match exactly.
    // Still, floating-point differences may lead to tiny mismatches, so we fallback to interpolation.
    private const double QRelativeTolerance = 1e-6;
    private const double QAbsoluteTolerance = 1e-12;

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

        var xRef = datasets[0].X;
        var n = xRef.Length;
        if (n == 0)
            return FluentResults.Result.Fail<string>("Cannot average: empty Q grid.");

        var count = datasets.Count;
        var ySum = new double[n];

        for (var k = 0; k < count; k++)
        {
            var xCur = datasets[k].X;
            var yCur = datasets[k].Y;

            var aligned = xCur.Length == n && AreQGridsAligned(xRef, xCur);
            var yOnRef = aligned ? yCur : InterpolateToGrid(xCur, yCur, xRef);

            for (var i = 0; i < n; i++)
                ySum[i] += yOnRef[i];
        }

        for (var i = 0; i < n; i++)
            ySum[i] /= count;

        var averageDataset = new Dataset
        {
            id = $"Average (n={count})",
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

    private static bool AreQGridsAligned(double[] xRef, double[] xCur)
    {
        var n = xRef.Length;
        for (var i = 0; i < n; i++)
        {
            var refVal = xRef[i];
            var curVal = xCur[i];
            var tol = Math.Max(Math.Abs(refVal), Math.Abs(curVal)) * QRelativeTolerance + QAbsoluteTolerance;
            if (Math.Abs(curVal - refVal) > tol)
                return false;
        }
        return true;
    }

    // Simple linear interpolation of intensity values onto a reference Q grid.
    // Assumes x arrays are sorted in ascending order.
    private static double[] InterpolateToGrid(double[] x, double[] y, double[] xQuery)
    {
        if (x.Length != y.Length)
            throw new InvalidOperationException("Cannot average: dataset X/Y length mismatch.");

        if (x.Length < 2)
            throw new InvalidOperationException("Cannot average: interpolation requires at least 2 Q points.");

        // Ensure monotonic increasing Q.
        for (var i = 0; i < x.Length - 1; i++)
        {
            if (x[i + 1] < x[i])
                throw new InvalidOperationException("Cannot average: Q grid is not monotonic.");
        }

        var result = new double[xQuery.Length];

        var j = 0;
        var xMin = x[0];
        var xMax = x[^1];
        for (var i = 0; i < xQuery.Length; i++)
        {
            var q = xQuery[i];
            if (q < xMin - QAbsoluteTolerance || q > xMax + QAbsoluteTolerance)
                throw new InvalidOperationException("Cannot average: Q range mismatch across datasets.");

            while (j < x.Length - 2 && x[j + 1] < q)
                j++;

            var x0 = x[j];
            var x1 = x[j + 1];
            var y0 = y[j];
            var y1 = y[j + 1];

            if (Math.Abs(x1 - x0) <= QAbsoluteTolerance)
            {
                // Degenerate segment.
                result[i] = y0;
                continue;
            }

            var t = (q - x0) / (x1 - x0);
            result[i] = y0 + t * (y1 - y0);
        }

        return result;
    }
}
