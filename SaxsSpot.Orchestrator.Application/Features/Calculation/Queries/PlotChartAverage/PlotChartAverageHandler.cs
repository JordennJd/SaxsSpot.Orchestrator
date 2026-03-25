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
    private const double QRelativeTolerance = 1e-5;
    private const double QAbsoluteTolerance = 1e-9;

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

        // We average only calculations that have Q grids aligned within tolerance.
        // This avoids hard-failing when user selected a parameter group that still contains
        // tiny numerical differences in Q.
        var bestRefIndex = -1;
        var alignedIndices = new List<int>();

        for (var refIndex = 0; refIndex < datasets.Count; refIndex++)
        {
            var xRefCandidate = datasets[refIndex].X;
            var nCandidate = xRefCandidate.Length;

            var indicesForThisRef = new List<int> { refIndex };
            for (var otherIndex = 0; otherIndex < datasets.Count; otherIndex++)
            {
                if (otherIndex == refIndex) continue;

                var xCur = datasets[otherIndex].X;
                if (xCur.Length != nCandidate)
                    continue;

                if (AreQGridsAligned(xRefCandidate, xCur))
                    indicesForThisRef.Add(otherIndex);
            }

            if (indicesForThisRef.Count > alignedIndices.Count)
            {
                alignedIndices = indicesForThisRef;
                bestRefIndex = refIndex;
            }
        }

        if (alignedIndices.Count == 0)
            return FluentResults.Result.Fail<string>("Cannot average: no aligned Q grids found.");

        if (alignedIndices.Count < 2)
            return FluentResults.Result.Fail<string>(
                $"Cannot average: only {alignedIndices.Count} calculation(s) have Q grids aligned within tolerance. " +
                $"Select another calculation group.");

        var xRef = datasets[bestRefIndex].X;
        var n = xRef.Length;
        var count = alignedIndices.Count;

        var ySum = new double[n];
        for (var i = 0; i < n; i++)
        {
            var sum = 0.0;
            for (var k = 0; k < count; k++)
            {
                var idx = alignedIndices[k];
                sum += datasets[idx].Y[i];
            }
            ySum[i] = sum / count;
        }

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
}
