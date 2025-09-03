using Confluent.Kafka;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.Orchestrator.Application.Features.Calculation.Command;
using SaxsSpot.Orchestrator.Contracts.Messages;

namespace SaxsSpot.Orchestrator.Kafka.Consumers;

/// <summary>
/// Result consumer for processing scattering calculation results
/// </summary>
public class CalculationResultConsumer(IMediator mediator, ILogger<CalculationResultConsumer> logger) : IConsumer<ScatteringResult>
{
    public async Task Consume(ConsumeContext<ScatteringResult> context)
    {
        logger.LogInformation("Starting to process scattering result with ID: {ResultId}", context.Message.Request.RequestId);
        
        try
        {
            var result = await mediator.Send(new SaveCalculationCommand(context.Message));
            if (result.IsFailed)
            {
                throw new InvalidOperationException($"Failed to save scattering result for {context.Message.Request.RequestId}");
            }
            logger.LogInformation("Successfully processed and saved scattering result with ID: {ResultId}", 
                context.Message.Request.RequestId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing scattering result with ID: {ResultId}", 
                context.Message.Request.RequestId);
            throw;
        }
    }
}