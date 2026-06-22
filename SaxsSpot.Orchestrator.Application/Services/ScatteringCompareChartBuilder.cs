using SaxsSpot.NanoSystemService.Sdk.Interfaces;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Application.Services;

public class ScatteringCompareChartBuilder(
    ICalculationStorage legacyStorage,
    ICalculateObjectStorage legacyObjectStorage,
    INanosystemServiceApiClient nanosystemClient)
{
    public async Task<List<Dataset>> BuildDatasetsAsync(
        IReadOnlyList<Guid> legacyCalculationIds,
        IReadOnlyList<Guid> nanoScatteringIds,
        bool averageLegacy,
        bool averageNano,
        CancellationToken cancellationToken)
    {
        var chartDatasets = new List<Dataset>();

        if (legacyCalculationIds.Count > 0)
        {
            var legacyDataset = await BuildLegacyDataset(legacyCalculationIds, averageLegacy, cancellationToken);
            if (legacyDataset is not null)
            {
                chartDatasets.Add(legacyDataset);
            }
        }

        if (nanoScatteringIds.Count > 0)
        {
            var nanoDataset = await BuildNanoDataset(nanoScatteringIds, averageNano, cancellationToken);
            if (nanoDataset is not null)
            {
                chartDatasets.Add(nanoDataset);
            }
        }

        return chartDatasets;
    }

    private async Task<Dataset?> BuildLegacyDataset(
        IReadOnlyList<Guid> ids,
        bool average,
        CancellationToken cancellationToken)
    {
        var datasets = new List<(double[] X, double[] Y)>();
        var calculations = await legacyStorage.WhereAsync(x => ids.Contains(x.Id));

        foreach (var calculation in calculations)
        {
            var points = new List<IntensityResult>();
            await foreach (var point in legacyObjectStorage.Load(calculation.ObjectId, cancellationToken)
                               .WithCancellation(cancellationToken))
            {
                points.Add(point);
            }

            if (points.Count == 0)
            {
                continue;
            }

            datasets.Add((points.Select(p => p.QVector).ToArray(), points.Select(p => p.Intensity).ToArray()));
        }

        return BuildCombinedDataset(datasets, average, "Legacy (worker)");
    }

    private async Task<Dataset?> BuildNanoDataset(
        IReadOnlyList<Guid> ids,
        bool average,
        CancellationToken cancellationToken)
    {
        var datasets = new List<(double[] X, double[] Y)>();

        foreach (var id in ids)
        {
            await using var stream = await nanosystemClient.DownloadScatteringCalculation(id, cancellationToken);
            using var reader = new StreamReader(stream);
            var points = new List<(double Q, double I)>();

            while (await reader.ReadLineAsync(cancellationToken) is { } line)
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 &&
                    double.TryParse(parts[0], out var q) &&
                    double.TryParse(parts[1], out var intensity))
                {
                    points.Add((q, intensity));
                }
            }

            if (points.Count == 0)
            {
                continue;
            }

            datasets.Add((points.Select(p => p.Q).ToArray(), points.Select(p => p.I).ToArray()));
        }

        return BuildCombinedDataset(datasets, average, "SAXS (new)");
    }

    private static Dataset? BuildCombinedDataset(
        IReadOnlyList<(double[] X, double[] Y)> datasets,
        bool average,
        string label)
    {
        if (datasets.Count == 0)
        {
            return null;
        }

        if (!average || datasets.Count == 1)
        {
            return new Dataset
            {
                id = label,
                x = datasets[0].X,
                y = datasets[0].Y
            };
        }

        var xRef = datasets[0].X;
        var n = xRef.Length;
        var ySum = new double[n];

        for (var i = 0; i < n; i++)
        {
            var sum = 0.0;
            for (var k = 0; k < datasets.Count; k++)
            {
                if (datasets[k].Y.Length != n)
                {
                    return null;
                }

                sum += datasets[k].Y[i];
            }

            ySum[i] = sum / datasets.Count;
        }

        return new Dataset
        {
            id = $"{label} average (n={datasets.Count})",
            x = (double[])xRef.Clone(),
            y = ySum
        };
    }
}
