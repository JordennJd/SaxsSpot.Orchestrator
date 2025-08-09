using MassTransit;
using SaxsSpot.Orchestrator.Contracts.Messages;

namespace SaxsSpot.Orchestrator.Kafka.Consumers;

/// <summary>
/// result consumer
/// </summary>
public class CalculationResultConsumer : IConsumer<ScatteringResult>
{
    public Task Consume(ConsumeContext<ScatteringResult> context)
    {
        throw new NotImplementedException();
    }
}