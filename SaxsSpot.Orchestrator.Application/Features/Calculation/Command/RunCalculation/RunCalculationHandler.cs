using System.Text.Json;
using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunCalculation;

public class RunCalculationHandler(ITopicProducer<CalculateScatteringRequest> producer,
    IJobServiceClient jobServiceClient, ILogger<RunCalculationHandler> logger) : IRequestHandler<RunCalculationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(RunCalculationCommand request, CancellationToken cancellationToken)
    {
        var operationId = Guid.NewGuid();
        logger.LogInformation("Starting calculation with id {id}", operationId);
        try
        {
            request.RequestId = operationId.ToString();

            await producer.Produce(request, cancellationToken);
            await jobServiceClient.CreateJobAsync
            (new JobModels.CreateJobQuery(operationId.ToString(),
                "calculate-scattering", "calculate scatteing message produced by the SaxsSpot", JsonSerializer.Serialize(request)));

            return FluentResults.Result.Ok(operationId);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error during calculation start with id {id} error: {ex}", operationId, ex.ToString());
            return FluentResults.Result.Fail<Guid>($"Error during calculation start");
        }
    }
}