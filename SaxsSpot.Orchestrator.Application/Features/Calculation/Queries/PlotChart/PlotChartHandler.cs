using System.Net.Http.Json;
using System.Text.Json;
using FluentResults;
using MassTransit.Internals;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Contracts.Enums;
using SaxsSpot.Orchestrator.Contracts.ExternalDto;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Queries.PlotChart
{
    public class PlotChartHandler(ICalculationStorage storage, ICalculateObjectStorage objectStorage,
        IConfiguration configuration, ILogger<PlotChartHandler> logger
    ) : IRequestHandler<PlotChartRequest, IResult<string>>
    {
        public async Task<IResult<string>> Handle(PlotChartRequest request, CancellationToken cancellationToken)
        {
            var calculations = await storage.WhereAsync(x => request.CalculatesId.Contains(x.Id));
        
            var plotRequest = new PlotRequest
            {
                title = request.ChartTitle,
                x_label = request.XAxis,
                y_label = request.YAxis,
                x_log_scale = request.ScaleMethodsX == SpaceMethod.Log,
                y_log_scale = request.ScaleMethodsY == SpaceMethod.Log,
            };
		
            var datasets = new List<Dataset>();
		
            foreach(var calculation in calculations)
            {
                var dataset = await objectStorage.Load(calculation.ObjectId, cancellationToken).ToListAsync(cancellationToken);

                datasets.Add(new Dataset(){x = dataset.Select(x => x.QVector).ToArray(),
                    y = dataset.Select(x => x.Intensity).ToArray(), id = calculation.Id.ToString()});	
            }
            datasets.ForEach(x => x.SortByX());
            plotRequest.datasets = datasets.ToArray();
            using var client = new HttpClient();
	
            try
            {
                var a = JsonSerializer.Serialize(plotRequest);
                var response = await client.PostAsJsonAsync(configuration.GetValue<string>("chart:uri")+"/plot", plotRequest, cancellationToken: cancellationToken);
                response.EnsureSuccessStatusCode();
                var htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return FluentResults.Result.Ok(htmlContent);
            }
            catch (Exception e)
            {
                logger.LogInformation(e.Message);
                return FluentResults.Result.Fail<string>("Build chart failed");
            }    
        }
    
    }
}