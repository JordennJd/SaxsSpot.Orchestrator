using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;
using SaxsSpot.Orchestrator.Domain.Entities;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChartPng;

public class PlotChartPngHandler(
    ICalculationStorage storage,
    ICalculateObjectStorage objectStorage,
    IChartService chartService
) : IRequestHandler<PlotChartPngRequest, IResult<string>>
{
    public async Task<IResult<string>> Handle(PlotChartPngRequest request, CancellationToken cancellationToken)
    {
        var calculations = await storage.WhereAsync(x => request.CalculatesId.Contains(x.Id));

        var datasets = new List<Dataset>();
        foreach (var calculation in calculations)
        {
            var datasetPoints = new List<IntensityResult>();
            await foreach (var p in objectStorage.Load(calculation.ObjectId, cancellationToken)
                               .WithCancellation(cancellationToken))
            {
                datasetPoints.Add(p);
            }

            datasets.Add(new Dataset
            {
                id = calculation.Id.ToString(),
                x = datasetPoints.Select(p => p.QVector).ToArray(),
                y = datasetPoints.Select(p => p.Intensity).ToArray()
            });
        }

        return await chartService.BuildChartPngAsync(
            request.ChartTitle,
            request.XAxis,
            request.YAxis,
            datasets.ToArray(),
            request.ScaleMethodsX,
            request.ScaleMethodsY,
            cancellationToken);
    }
}

