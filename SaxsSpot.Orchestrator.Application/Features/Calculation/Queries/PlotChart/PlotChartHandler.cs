using FluentResults;
using MassTransit.Internals;
using MediatR;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChart
{
    public class PlotChartHandler(ICalculationStorage storage, ICalculateObjectStorage objectStorage,
        IChartService chartService
    ) : IRequestHandler<PlotChartRequest, IResult<string>>
    {
        public async Task<IResult<string>> Handle(PlotChartRequest request, CancellationToken cancellationToken)
        {
            var calculations = await storage.WhereAsync(x => request.CalculatesId.Contains(x.Id));
		
            var datasets = new List<Dataset>();
		
            var calcList = calculations.ToList();
            for (var i = 0; i < calcList.Count; i++)
            {
                var calculation = calcList[i];
                var dataset = await objectStorage.Load(calculation.ObjectId, cancellationToken).ToListAsync(cancellationToken);

                datasets.Add(new Dataset(){x = dataset.Select(x => x.QVector).ToArray(),
                    y = dataset.Select(x => x.Intensity).ToArray(),
                    id = calcList.Count == 1 ? "By model" : $"By model ({i + 1})"});	
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
    }
}