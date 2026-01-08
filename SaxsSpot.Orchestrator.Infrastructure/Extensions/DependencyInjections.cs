using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemService.Sdk.Extensions;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Domain.NanosystemApi;
using SaxsSpot.Orchestrator.Infrastructure.DbContexts;
using SaxsSpot.Orchestrator.Infrastructure.ExternalApis;
using SaxsSpot.Orchestrator.Infrastructure.Storages;

namespace SaxsSpot.Orchestrator.Infrastructure.Extensions;

public static class DependencyInjections
{
    public static IServiceCollection AddCalculationServiceStorage(this IServiceCollection services)
    {
        return services.AddDbContext<CalculationDbContext>()
            .AddDbContext<NanosystemSeriesDbContext>()
            .AddScoped<INanosystemApi, NanosystemApi>()
            .AddScoped<ICalculationStorage, CalculationStorage>()
            .AddScoped<ICalculateObjectStorage, CalculateObjectStorage>()
            .AddScoped<INanoSystemSeriesStorage, NanoSystemSeriesStorage>();
    }

    public static IServiceCollection AddIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddNanoSystemServices(configuration.GetSection("nanosystem-service").GetValue<string>("url"));

    }

}