using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Sdk.Extensions;
using SaxsSpot.Orchestrator.Application.Interfaces;
using SaxsSpot.Orchestrator.Application.Services;

namespace SaxsSpot.Orchestrator.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var domain = AppDomain.CurrentDomain.GetAssemblies();
        
        return services
            .AddLogging(cfg => cfg.AddConsole())
            .AddScoped<IChartService, ChartService>()
            .AddScoped<ScatteringCompareChartBuilder>()
            .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssemblies(domain);
                    // cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                })
            .AddAutoMapper(cfg => cfg.AddMaps(domain))
            .AddValidatorsFromAssemblies(domain);
    }
}