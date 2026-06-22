using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Application.Mapping;
using SaxsSpot.Orchestrator.Application.Services;

namespace SaxsSpot.Orchestrator.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssembly = typeof(CalculationProfile).Assembly;

        return services
            .AddLogging(cfg => cfg.AddConsole())
            .AddScoped<IChartService, ChartService>()
            .AddScoped<ScatteringCompareChartBuilder>()
            .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(applicationAssembly);
                    // cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                })
            .AddAutoMapper(cfg => cfg.AddMaps(applicationAssembly))
            .AddValidatorsFromAssembly(applicationAssembly);
    }
}