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
        IConfiguration configuration)
    {
        var kafkaConfiguration = configuration.GetSection("kafka").Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);
        
        services.AddMassTransit(x =>
        {
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            x.AddRider(rider =>
            {
                rider.AddConsumer<CalculationResultConsumer>();

                rider.AddProducer<CalculateScatteringRequest>(kafkaConfiguration.Topic);

                rider.UsingKafka((context, k) =>
                {
                    k.Host(kafkaConfiguration.Brokers);

                    k.TopicEndpoint<ScatteringResult>(kafkaConfiguration.ResultTopic, kafkaConfiguration.Group, e =>
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