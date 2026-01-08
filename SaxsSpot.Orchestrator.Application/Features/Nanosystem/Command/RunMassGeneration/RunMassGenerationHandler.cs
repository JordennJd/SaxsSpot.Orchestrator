using System.Text.Json;
using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Nanosystem.Command.RunMassGeneration;

public class RunMassGenerationHandler(
    ITopicProducer<RunGenerationRequest> producer,
    IJobServiceClient jobServiceClient,
    ILogger<RunMassGenerationHandler> logger) : IRequestHandler<RunMassGenerationCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(RunMassGenerationCommand request, CancellationToken cancellationToken)
    {
        var operationId = Guid.NewGuid();
        
        logger.LogInformation("Starting mass nanosystem generation with operation id {OperationId}, count: {Count}", 
            operationId, request.Parameters.Options.Count);
        
        try
        {
            // Create main job for mass generation
            var createResult = await jobServiceClient.CreateJobAsync(
                new JobModels.CreateJobQuery(
                    operationId.ToString(),
                    "nanosystem-mass-generation",
                    $"Nanosystem mass generation: {request.Parameters.Options.Count} tasks",
                    JsonSerializer.Serialize(request)));
            
            if (createResult.IsSuccessful is false)
            {
                throw new InvalidOperationException(
                    $"Main job not created with id {operationId} with error on remote server {createResult.ErrorMessage}");
            }
            
            // Start main job
            var startResult = await jobServiceClient.StartJobAsync(new JobModels.StartJobQuery(operationId.ToString()));
            
            if (startResult.IsSuccessful is false)
            {
                throw new InvalidOperationException(
                    $"Main job not started with id {operationId} with error on remote server {startResult.ErrorMessage}");
            }
            
            // Generate series ID that will be used for all messages in this mass generation
            // Series will be created/updated in NanoSystemService when processing messages
            var seriesId = Guid.NewGuid();
            logger.LogInformation("Using series id {SeriesId} for mass generation", seriesId);
            
            // Send each option as a separate message to Kafka and create child job for each
            for (int i = 0; i < request.Parameters.Options.Count; i++)
            {
                var option = request.Parameters.Options[i];
                var messageOperationId = Guid.NewGuid();
                
                // Create child job (only CreateJob, no Start/Complete)
                await jobServiceClient.CreateJobAsync(
                    new JobModels.CreateJobQuery(
                        messageOperationId.ToString(),
                        "nanosystem-generation",
                        $"Nanosystem generation task {i + 1}/{request.Parameters.Options.Count}",
                        JsonSerializer.Serialize(option)));
                
                var message = new RunGenerationRequest
                {
                    OperationId = messageOperationId,
                    SeriesId = seriesId,
                    Parameters = option
                };
                
                await producer.Produce(message, cancellationToken);
                logger.LogInformation("Nanosystem generation message {Index}/{Total} produced with operation id {MessageOperationId}", 
                    i + 1, request.Parameters.Options.Count, messageOperationId);
            }
            
            // Complete main job after all messages are sent
            var completeResult = await jobServiceClient.CompleteJobAsync(
                new JobModels.CompleteJobQuery(
                    operationId.ToString(),
                    $"Mass nanosystem generation completed: {request.Parameters.Options.Count} tasks sent to Kafka"));
            
            if (completeResult.IsSuccessful is false)
            {
                logger.LogWarning("Main job completed with id {OperationId} but job service returned error: {ErrorMessage}", 
                    operationId, completeResult.ErrorMessage);
            }
            
            logger.LogInformation("All nanosystem generation messages produced with main operation id {OperationId}", operationId);

            return FluentResults.Result.Ok(operationId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during mass nanosystem generation start with operation id {OperationId}", operationId);
            
            // Try to complete main job with error status
            try
            {
                await jobServiceClient.CompleteJobAsync(new JobModels.CompleteJobQuery(
                    operationId.ToString(),
                    $"Mass nanosystem generation failed: {ex.Message}",
                    IsFailed: true));
            }
            catch (Exception completeEx)
            {
                logger.LogError(completeEx, "Failed to complete main job with error status");
            }
            
            return FluentResults.Result.Fail<Guid>($"Error during mass nanosystem generation start: {ex.Message}");
        }
    }
}
