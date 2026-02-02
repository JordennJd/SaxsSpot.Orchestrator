using System.Text.Json;
using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Nanosystem.Command.RunGeneration;

public class RunGenerationHandler(
    ITopicProducer<RunGenerationRequest> producer,
    IJobServiceClient jobServiceClient,
    ILogger<RunGenerationHandler> logger) : IRequestHandler<RunGenerationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(RunGenerationCommand request, CancellationToken cancellationToken)
    {
        var operationId = request.OperationId == Guid.Empty ? Guid.NewGuid() : request.OperationId;
        
        logger.LogInformation("Starting nanosystem generation with operation id {OperationId}", operationId);
        
        try
        {
            await jobServiceClient.CreateJobAsync(
                new JobModels.CreateJobQuery(
                    operationId.ToString(),
                    "nanosystem-generation",
                    "Nanosystem generation message produced by the SaxsSpot",
                    JsonSerializer.Serialize(request)));
            
            var message = new RunGenerationRequest
            {
                OperationId = operationId,
                Parameters = request.Parameters
            };
            
            await producer.Produce(message, cancellationToken);
            logger.LogInformation("Nanosystem generation message produced with operation id {OperationId}", operationId);

            return FluentResults.Result.Ok(operationId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during nanosystem generation start with operation id {OperationId}", operationId);
            return FluentResults.Result.Fail<Guid>($"Error during nanosystem generation start: {ex.Message}");
        }
    }
}
