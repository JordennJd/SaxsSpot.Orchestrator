using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Orchestrator.Kafka.Consumers;

namespace SaxsSpot.Orchestrator.Kafka.Extensions;

public static class KafkaMassTransitExtensions
{
    public static IServiceCollection AddKafkaEventing(
        this IServiceCollection services,
        KafkaConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            x.AddRider(rider =>
            {
                rider.AddConsumer<CalculationResultConsumer>();

                rider.AddProducer<string, CalculateScatteringRequest>(configuration.Topic);

                rider.UsingKafka((context, k) =>
                {
                    k.Host(configuration.Brokers);

                    // Настройка TopicEndpoint
                    k.TopicEndpoint<ScatteringResult>(configuration.ResultTopic, configuration.Group, e =>
                    {
                        e.ConfigureConsumer<CalculationResultConsumer>(context);
                        e.CreateIfMissing();
                    });
                });
            });
        });

        return services;
    }
}